using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Newtonsoft.Json;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for KlantenService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class KlantenService : System.Web.Services.WebService
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;

        [WebMethod]
        public string checkExists(string Naam, string Adres, string Postcode, string Woonplaats, string Email, string Wachtwoord, string Telefoon, string Bevestigingsguid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Controleer of het e-mailadres bestaat met Bevestigd = 'Nee'
                string query = "SELECT Bevestigd FROM Klanten WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        if (result.ToString() == "Nee")
                        {
                            return "niet_bevestigd";
                        }
                        return "ww_vergeten";
                    }
                    else
                    {
                        // Voeg nieuw record toe
                        string insertQuery = "INSERT INTO Klanten (Naam, Adres, Postcode, Woonplaats, Email, Wachtwoord, Telefoon, Bevestigingsguid, Bevestigd) VALUES (@Naam, @Adres, @Postcode, @Woonplaats, @Email, @Wachtwoord, @Telefoon, @Bevestigingsguid, @Bevestigd)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@Naam", Naam);
                            insertCmd.Parameters.AddWithValue("@Adres", Adres);
                            insertCmd.Parameters.AddWithValue("@Postcode", Postcode);
                            insertCmd.Parameters.AddWithValue("@Woonplaats", Woonplaats);
                            insertCmd.Parameters.AddWithValue("@Email", Email);
                            insertCmd.Parameters.AddWithValue("@Wachtwoord", Wachtwoord);
                            insertCmd.Parameters.AddWithValue("@Telefoon", Telefoon);
                            insertCmd.Parameters.AddWithValue("@Bevestigingsguid", Bevestigingsguid);
                            insertCmd.Parameters.AddWithValue("@Bevestigd", "Nee");
                            insertCmd.ExecuteNonQuery();
                        }
                        return "record_ingevoerd";
                    }
                }
            }
        }

        [WebMethod]
        public Dictionary<string, object> FetchUuid(string email)
        {
            var response = new Dictionary<string, object>
            {
                { "success", false },
                { "message", "Het e-mailadres bestaat niet." },
                { "uuid", null }
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Bevestigingsguid FROM Klanten WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        string uuid = result.ToString();
                        response["success"] = true;
                        response["message"] = "Het UUID is gevonden.";
                        response["uuid"] = uuid;

                        // Verstuur het UUID via e-mail
                        SendEmail(email, uuid);
                    }
                }
            }

            return response;
        }

        private void SendEmail(string email, string uuid)
        {
            // SMTP-configuratie
            using (SmtpClient smtp = new SmtpClient("mail.aspworld.nl"))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("webshop@aspworld.nl", "^9ffo75D6");
                smtp.EnableSsl = false;
                smtp.Port = 25;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("webshop@aspworld.nl"),
                    Subject = "Verificatie UUID",
                    Body = $"Beste gebruiker,\n\nHier is jouw UUID voor verificatie:\n\n{uuid}\n\nKopieer bovenstaande code en voer deze op de website in om je account te bevestigen. Met vriendelijke groet,\nWebshop team.",
                };
                mailMessage.To.Add(email);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = false;
                smtp.Send(mailMessage);
            }
        }

        [WebMethod]
        public Dictionary<string, object> VerifieerKlant(string email, string verificatieCode)
        {
            var response = new Dictionary<string, object>
            {
                { "success", false }
            };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Klanten SET Bevestigd = 'Ja' WHERE Email = @Email AND Bevestigingsguid = @VerificatieCode";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@VerificatieCode", verificatieCode);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        response["success"] = true;
                    }
                }
            }
            return response;
        }

        [WebMethod]
        public string GetWachtwoord(string email, string bevestigingsGuid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Wachtwoord FROM Klanten WHERE Email = @Email AND Bevestigingsguid = @BevestigingsGuid";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@BevestigingsGuid", bevestigingsGuid);
                    connection.Open();

                    var wachtwoord = command.ExecuteScalar();
                    return wachtwoord != null ? wachtwoord.ToString() : "Geen klant gevonden.";
                }
            }
        }

        [WebMethod]
        public Dictionary<string, object> Login(string Email, string Wachtwoord)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT Id, Bevestigd FROM Klanten WHERE Email = @Email AND Wachtwoord = @Wachtwoord";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Wachtwoord", Wachtwoord);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool bevestigd = reader["Bevestigd"].ToString() == "Ja";
                            response["success"] = bevestigd;
                            response["id"] = reader["Id"];
                            response["message"] = bevestigd ? "ingelogd" : "niet_bevestigd";
                        }
                        else
                        {
                            response["success"] = false;
                            response["message"] = "ww_vergeten";
                        }
                    }
                }
            }

            return response;
        }

        [WebMethod]
        public string VoerKlantIDin(string bestelguid, int klantid)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Bezorging_en_betaling SET KlantId = @KlantId WHERE Bestelguid = @Bestelguid";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KlantId", klantid);
                        command.Parameters.AddWithValue("@Bestelguid", bestelguid);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0
                            ? "update_success"
                            : "no_record_updated";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Fout bij updaten: {ex.Message}";
            }
        }

        [WebMethod]
        public string GetKlantById(int Id)
        {
            var klantData = new
            {
                Naam = "",
                Adres = "",
                Postcode = "",
                Woonplaats = "",
                Email = "",
                Wachtwoord = "",
                Telefoon = ""
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Naam, Adres, Postcode, Woonplaats, Email, Wachtwoord, Telefoon FROM Klanten WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        klantData = new
                        {
                            Naam = reader["Naam"].ToString(),
                            Adres = reader["Adres"].ToString(),
                            Postcode = reader["Postcode"].ToString(),
                            Woonplaats = reader["Woonplaats"].ToString(),
                            Email = reader["Email"].ToString(),
                            Wachtwoord = reader["Wachtwoord"].ToString(),
                            Telefoon = reader["Telefoon"].ToString()
                        };
                    }
                }
            }
            return JsonConvert.SerializeObject(klantData, Newtonsoft.Json.Formatting.Indented);
            //return new JavaScriptSerializer().Serialize(klantData);
        }

        [WebMethod]
        public string updateKlant(int Id, string Naam, string Adres, string Postcode, string Woonplaats, string Email, string Wachtwoord, string Telefoon)
        {
            string query = "UPDATE Klanten SET Naam=@Naam, Adres=@Adres, Postcode=@Postcode, Woonplaats=@Woonplaats, Wachtwoord=@Wachtwoord, Telefoon=@Telefoon WHERE Id=@Id AND Email=@Email";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@Email", Email);

                    cmd.Parameters.AddWithValue("@Naam", Naam);
                    cmd.Parameters.AddWithValue("@Adres", Adres);
                    cmd.Parameters.AddWithValue("@Postcode", Postcode);
                    cmd.Parameters.AddWithValue("@Woonplaats", Woonplaats);
                    cmd.Parameters.AddWithValue("@Wachtwoord", Wachtwoord);
                    cmd.Parameters.AddWithValue("@Telefoon", Telefoon);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0 ? "update_success" : "update_failure!";
                    }
                    catch (Exception ex)
                    {
                        return "Fout: " + ex.Message;
                    }
                }
            }
        }

        [WebMethod]
        public decimal GetTotaalbedrag(string Bestelguid, int KlantId)
        {
            decimal totaalbedrag = 0;

            // Maak verbinding met de database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Totaalbedrag, Bezorgtarief FROM Bezorging_en_betaling WHERE Bestelguid = @Bestelguid AND KlantId = @KlantId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);
                    cmd.Parameters.AddWithValue("@KlantId", KlantId);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        totaalbedrag = Convert.ToDecimal(result);
                }
            }

            return totaalbedrag;
        }

    }
}
