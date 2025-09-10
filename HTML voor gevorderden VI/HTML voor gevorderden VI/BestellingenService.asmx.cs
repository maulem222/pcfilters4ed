using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for BestellingenService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BestellingenService : System.Web.Services.WebService
    {

        [WebMethod]
        public string getBestellingen(string sql)
        {
            string query = sql;
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public void BestelProdukt(string pid, string prijs, string produktnaam, string produktafbeelding, string aantal, string gewicht, string produktlink, string bestelguid, string tabelnaam, string besteldatum)
        {
            int Id = 0;
            int Aantal;
            int nieuwe_aantal = 0;

            string query = "SELECT * FROM Bestellingen WHERE Pid=@pid AND Tabelnaam=@tab AND Bestelguid=@guid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.Parameters.AddWithValue("@tab", tabelnaam);
            cmd.Parameters.AddWithValue("@guid", bestelguid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // doe dit alleen indien bestelling al bestaat
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Id = Convert.ToInt32(row["Id"]);
                    Aantal = Convert.ToInt32(row["Aantal"]);
                    nieuwe_aantal = Aantal + Convert.ToInt32(aantal);
                }
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string updatequery = "UPDATE Bestellingen SET Aantal=@aantal WHERE Id=@id";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(updatequery, con);
                command.Parameters.AddWithValue("@aantal", nieuwe_aantal);
                command.Parameters.AddWithValue("@id", Convert.ToInt32(Id));
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
            else // bestelling bestaat nog niet
            {
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string insertquery = "INSERT INTO Bestellingen (Pid, Prijs, Produktnaam, Produktafbeelding, Aantal, Gewicht, ProduktLink, Bestelguid," +
                    " Tabelnaam, Besteldatum) VALUES " +
                    "(@pid, @prijs, @prodnaam, @prodafb, @aantal, @gewicht, @prodlink, @guid, @tabelnaam, @bestdatum)";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(insertquery, con);
                command.Parameters.AddWithValue("@pid", pid);
                command.Parameters.AddWithValue("@prijs", prijs);
                command.Parameters.AddWithValue("@prodnaam", produktnaam);
                command.Parameters.AddWithValue("@prodafb", produktafbeelding);
                command.Parameters.AddWithValue("@aantal", aantal);
                command.Parameters.AddWithValue("@gewicht", gewicht);
                command.Parameters.AddWithValue("@prodlink", produktlink);
                command.Parameters.AddWithValue("@guid", bestelguid);
                command.Parameters.AddWithValue("@tabelnaam", tabelnaam);
                command.Parameters.AddWithValue("@bestdatum", besteldatum);
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
        }

        [WebMethod]
        public void Vervang_aantal(string pid, string aantal, string bestelguid, string tabelnaam)
        {
            int Id = 0;

            string query = "SELECT * FROM Bestellingen WHERE Pid=@pid AND Tabelnaam=@tab AND Bestelguid=@guid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.Parameters.AddWithValue("@tab", tabelnaam);
            cmd.Parameters.AddWithValue("@guid", bestelguid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // 
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Id = Convert.ToInt32(row["Id"]);
                }
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string updatequery = "UPDATE Bestellingen SET Aantal=@aantal WHERE Id=@id";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(updatequery, con);
                command.Parameters.AddWithValue("@aantal", aantal);
                command.Parameters.AddWithValue("@id", Convert.ToInt32(Id));
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
        }

        [WebMethod]
        public void DELETEBestelling(string id)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Bestellingen WHERE Id=@id";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(deletequery, con);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
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
        public string DoesGuidExist(string guid)
        {
            string guidexists;
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand("DoesGuidExist", con);
            command.CommandType = CommandType.StoredProcedure;
            // input parameter
            command.Parameters.AddWithValue("@guid", guid);
            // output parameter
            SqlParameter outParam = new SqlParameter();
            outParam.SqlDbType = SqlDbType.NVarChar;
            outParam.Size = 10;
            outParam.ParameterName = "@guidexists";
            outParam.Value = "";
            outParam.Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(outParam);

            try
            {
                //outParam.Value = string.Empty;
                con.Open();
                command.ExecuteNonQuery();

                guidexists = (outParam.Value).ToString();

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
            return guidexists;
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
