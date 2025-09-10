using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for ProduktService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ProduktService : System.Web.Services.WebService
    {
        [WebMethod]
        public string getProducts(string sql)
        {
            string query = sql;
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public string getProduct(string sql)
        {
            string query = sql;
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        public int InsertedID;

        [WebMethod]
        public int InsertProdukt(string Produktnaam, string Catid)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlCommand command = new SqlCommand("INSERT_produktnaam_with_outputparameter_ID", con);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Produktnaam", Produktnaam);
            command.Parameters.AddWithValue("@Catid", Catid);
            command.Parameters.Add("@ID", SqlDbType.Int);
            command.Parameters["@ID"].Direction = ParameterDirection.Output;
            command.Connection = con;
            try
            {
                command.ExecuteNonQuery();
                InsertedID = command.Parameters["@ID"].Value != DBNull.Value ? (int)command.Parameters["@ID"].Value : 0;
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
            return InsertedID;
        }

        [WebMethod]
        public void VerwijderAlleProdukten()
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Produkten";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(deletequery, con);
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
        public void UPDATEProdgeg(string Id, string Produktomschrijving, string Produktafbeelding)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string updatequery = "UPDATE Produkten SET Produktomschr=@prodomschr, Produktafb=@prodafb WHERE Id=@id";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(updatequery, con);
            command.Parameters.AddWithValue("@prodomschr", Produktomschrijving);
            command.Parameters.AddWithValue("@prodafb", Produktafbeelding);
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

        [WebMethod]
        public void setProduktLink(string id, string produktlink, string sql)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string updatequery = sql;
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(updatequery, con);
            command.Parameters.AddWithValue("@prodlink", produktlink);
            command.Parameters.AddWithValue("@id", Convert.ToInt32(id));
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

        public class Produkt
        {
            public string Produktnaam;
        }

        public class ProdOmschrAfb
        {
            public string ProdOmschr;
            public string ProdAfb;
        }

        [WebMethod]
        public Produkt GetProduktnaam(string newID)
        {
            Produkt produkt = new Produkt();
            string query = "SELECT * FROM Produkten WHERE Id=@newID";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@newID", newID);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                produkt.Produktnaam = row["Produktnaam"].ToString();
            }
            return produkt;
        }

        [WebMethod]
        public ProdOmschrAfb GetOmschrAfb(string newID)
        {
            ProdOmschrAfb produkt = new ProdOmschrAfb();
            string query = "SELECT * FROM Produkten WHERE Id=@newID";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@newID", newID);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                produkt.ProdOmschr = row["Produktomschr"].ToString();
                produkt.ProdAfb = row["Produktafb"].ToString();
            }
            return produkt;
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
