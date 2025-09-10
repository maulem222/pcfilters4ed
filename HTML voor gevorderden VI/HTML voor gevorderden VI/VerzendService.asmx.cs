using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for VerzendService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class VerzendService : System.Web.Services.WebService
    {

        [WebMethod]
        public string getTariefgroepen(string sql)
        {
            string query = sql;
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public string getBezorgkosten(string sql)
        {
            string query = sql;
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public int getLevertijd(string sql)
        {
            int maxlevertijd;

            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            try
            {
                con.Open();
                maxlevertijd = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                // handle your error but don't trap it here..
                throw;
            }
            return maxlevertijd;
        }

        [WebMethod]
        public bool BestaatGuid(string guid)
        {
            bool guidexists;

            string query = "SELECT * FROM Bezorging_en_betaling WHERE Bestelguid=@guid;";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@guid", guid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // 
            if (dt.Rows.Count > 0)
            {
                guidexists = true;
            }
            else
            {
                guidexists = false;
            }
            return guidexists;
        }

        [WebMethod]
        public int UpdateBestelnummer()
        {
            int updatedbestnum;
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            SqlConnection con = new SqlConnection(con_str);
            con.Open();
            SqlCommand command = new SqlCommand("UpdateBestelnummer", con);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@incbestnum", SqlDbType.Int);
            command.Parameters["@incbestnum"].Direction = ParameterDirection.Output;
            try
            {
                command.ExecuteNonQuery();
                updatedbestnum = command.Parameters["@incbestnum"].Value != DBNull.Value ? (int)command.Parameters["@incbestnum"].Value : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return updatedbestnum;
        }

        [WebMethod]
        public void InsertBestelnummer(string guid, string bestelnummer)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string insertquery = "INSERT INTO Bezorging_en_betaling (Bestelguid, Bestelnummer) VALUES (@guid, @bestnum)";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(insertquery, con);
            command.Parameters.AddWithValue("@guid", guid);
            command.Parameters.AddWithValue("@bestnum", bestelnummer);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        [WebMethod]
        public void UpdateData(string guid, string besteldatum, string verzenddatum, string bezorgdatum, string bezorgmethode, string bezorgtarief, string totaalbedrag, string verzendkosten)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string updatequery = "UPDATE Bezorging_en_betaling SET Besteldatum=@bestd, Verzenddatum=@verzd, Bezorgdatum=@bezd, Bezorgmethode=@bezm, " +
                "Bezorgtarief=@beztar, Totaalbedrag=@tot, Verzendkosten=@verzk WHERE Bestelguid=@guid";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(updatequery, con);
            command.Parameters.AddWithValue("@bestd", besteldatum);
            command.Parameters.AddWithValue("@verzd", verzenddatum);
            command.Parameters.AddWithValue("@bezd", bezorgdatum);
            command.Parameters.AddWithValue("@bezm", bezorgmethode);
            command.Parameters.AddWithValue("@beztar", bezorgtarief);
            command.Parameters.AddWithValue("@tot", totaalbedrag);
            command.Parameters.AddWithValue("@verzk", verzendkosten);
            command.Parameters.AddWithValue("@guid", guid);
            try
            {
                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        [WebMethod]
        public string GetBezorgmethode(string guid)
        {
            string Bezorgmethode = "";

            string query = "SELECT Bezorgmethode FROM Bezorging_en_betaling WHERE Bestelguid=@guid;";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@guid", guid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // 
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Bezorgmethode = row["Bezorgmethode"].ToString();
                }
            }
            else
            {
                Bezorgmethode = "bezorgen";
            }
            return Bezorgmethode;
        }

        [WebMethod]
        public string UpdateVerzonden(string Verzonden, string Bestelguid)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string query = "UPDATE Bezorging_en_betaling SET Verzonden = @Verzonden WHERE Bestelguid = @Bestelguid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Verzonden", Verzonden);
                cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Update successful" : "No rows updated";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }


        [WebMethod]
        public string GetVerzonden(string Bestelguid)
        {
            string result = "";

            string query = "SELECT Verzonden FROM Bezorging_en_betaling WHERE Bestelguid = @Bestelguid";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // 
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result = row["Verzonden"].ToString();
                }
            }

            return result;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPakbonFactuur(string Bestelguid)
        {
            var result = new
            {
                pakbon = "",
                factuur = ""
            };

            string query = "SELECT Pakbon, Factuur FROM Bezorging_en_betaling WHERE Bestelguid = @Bestelguid";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // 
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result = new
                    {
                        pakbon = row["Pakbon"].ToString(),
                        factuur = row["Factuur"].ToString()
                    };
                }
            }

            return new JavaScriptSerializer().Serialize(result);
        }

        [WebMethod]
        public string UpdatePakbon(string Bestelguid)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string query = "UPDATE Bezorging_en_betaling SET Pakbon = 'Ja' WHERE Bestelguid = @Bestelguid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);
                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Update successful" : "No rows updated";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        [WebMethod]
        public string UpdateFactuur(string Bestelguid)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string query = "UPDATE Bezorging_en_betaling SET Factuur = 'Ja' WHERE Bestelguid = @Bestelguid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);
                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Update successful" : "No rows updated";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        private static DataSet GetData(SqlCommand cmd)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            using (SqlConnection con = new SqlConnection(con_str))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds);
                        return ds;
                    }
                }
            }
        }
    }
}
