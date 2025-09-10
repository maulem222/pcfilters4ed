using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for GegevensService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class GegevensService : System.Web.Services.WebService
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;

        [WebMethod]
        public void UpdateBedrijfsgegevens(Dictionary<string, string> gegevens)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE Bedrijfsgegevens SET
                Bedrijfsnaam = @Bedrijfsnaam,
                Adres = @Adres,
                Postcode = @Postcode,
                Plaats = @Plaats,
                Email = @Email,
                Telefoon = @Telefoon,
                KvKNummer = @KvKNummer,
                BTWNummer = @BTWNummer
                WHERE Code = 123";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Bedrijfsnaam", gegevens["Bedrijfsnaam"]);
                cmd.Parameters.AddWithValue("@Adres", gegevens["Adres"]);
                cmd.Parameters.AddWithValue("@Postcode", gegevens["Postcode"]);
                cmd.Parameters.AddWithValue("@Plaats", gegevens["Plaats"]);
                cmd.Parameters.AddWithValue("@Email", gegevens["Email"]);
                cmd.Parameters.AddWithValue("@Telefoon", gegevens["Telefoon"]);
                cmd.Parameters.AddWithValue("@KvKNummer", gegevens["KvKNummer"]);
                cmd.Parameters.AddWithValue("@BTWNummer", gegevens["BTWNummer"]);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, string> GetBedrijfsgegevens()
        {
            var gegevens = new Dictionary<string, string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Bedrijfsgegevens WHERE Code = 123";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    gegevens["Bedrijfsnaam"] = reader["Bedrijfsnaam"]?.ToString() ?? "";
                    gegevens["Adres"] = reader["Adres"]?.ToString() ?? "";
                    gegevens["Postcode"] = reader["Postcode"]?.ToString() ?? "";
                    gegevens["Plaats"] = reader["Plaats"]?.ToString() ?? "";
                    gegevens["Email"] = reader["Email"]?.ToString() ?? "";
                    gegevens["Telefoon"] = reader["Telefoon"]?.ToString() ?? "";
                    gegevens["KvKNummer"] = reader["KvKNummer"]?.ToString() ?? "";
                    gegevens["BTWNummer"] = reader["BTWNummer"]?.ToString() ?? "";
                }
            }

            return gegevens;
        }

        [WebMethod]
        public string getDomein()
        {
            string Domein = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT Domein FROM Gegevens WHERE Code=123";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Domein = reader["Domein"].ToString();
                }
            }

            return Domein;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetEmailGegevens()
        {
            string query = "SELECT Pakbonemail, Intfactuuremail FROM Gegevens WHERE Code=123";
            var result = new
            {
                Pakbonemail = "",
                Intfactuuremail = ""
            };

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        result = new
                        {
                            Pakbonemail = reader["Pakbonemail"].ToString(),
                            Intfactuuremail = reader["Intfactuuremail"].ToString()
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    return new JavaScriptSerializer().Serialize(new { error = ex.Message });
                }
            }

            return new JavaScriptSerializer().Serialize(result);
        }
    }
}
