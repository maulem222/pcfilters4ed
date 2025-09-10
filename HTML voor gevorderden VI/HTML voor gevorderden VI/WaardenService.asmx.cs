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
    /// Summary description for WaardenService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WaardenService : System.Web.Services.WebService
    {
        public class Wa
        {
            public string Id;
            public string Waarde;
        }

        static List<Wa> waardenlijst = new List<Wa> { };

        [WebMethod]
        public List<Wa> getWaarden(string Catid, string Eigenschap)
        {
            waardenlijst.Clear();
            string query = "SELECT * FROM Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Wa waarde = new Wa();
                waarde.Id = row["Id"].ToString();
                waarde.Waarde = row["Waarde"].ToString();
                waardenlijst.Add(waarde);
            }
            return waardenlijst;
        }

        [WebMethod]
        public Wa GetWaardeGegevens(string WaardeId)
        {
            string query = "SELECT * FROM Eigenschappen WHERE Id=@waid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@waid", Convert.ToInt32(WaardeId));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];

            Wa waarde = new Wa();
            foreach (DataRow row in dt.Rows)
            {
                waarde.Id = row["Id"].ToString();
                waarde.Waarde = row["Waarde"].ToString();
            }
            return waarde;
        }

        public class Message
        {
            public string Bericht;
        }

        [WebMethod]
        public Message INSERTWaarde(string Eigenschap, string Catid, string FilterElement, string Waarde)
        {
            string query = "SELECT * FROM Eigenschappen WHERE Eigenschap=@eig and Catid=@catid and Waarde=@waarde";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            cmd.Parameters.AddWithValue("@catid", Catid);
            cmd.Parameters.AddWithValue("@waarde", Waarde);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // doe dit alleen indien waarde niet al bestaat
            // retourneer naar gelang inv_succes of waarde_bestaat string

            Message mess = new Message();

            if (dt.Rows.Count > 0)
            {
                mess.Bericht = "waarde_bestaat";
            }
            else // Eigenschap bestaat nog niet
            {
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string insertquery = "INSERT INTO Eigenschappen (Eigenschap, Catid, FilterElement, Waarde) VALUES (@eig, @catid, @elem, @wa)";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(insertquery, con);
                command.Parameters.AddWithValue("@eig", Eigenschap);
                command.Parameters.AddWithValue("@catid", Catid);
                command.Parameters.AddWithValue("@elem", FilterElement);
                command.Parameters.AddWithValue("@wa", Waarde);
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
                mess.Bericht = "inv_succes";
            }
            return mess;
        }

        [WebMethod]
        public Message UPDATEWaarde(string Id, string Waarde, string Eigenschap, string Catid)
        {
            string query = "SELECT * FROM Eigenschappen WHERE Eigenschap=@eig and Catid=@catid and Waarde=@waarde";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            cmd.Parameters.AddWithValue("@catid", Catid);
            cmd.Parameters.AddWithValue("@waarde", Waarde);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // doe dit alleen indien waarde niet al bestaat
            // retourneer naar gelang inv_succes of waarde_bestaat string

            Message mess = new Message();

            if (dt.Rows.Count > 0)
            {
                mess.Bericht = "waarde_bestaat";
            }
            else // Eigenschap bestaat nog niet
            {
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string updatequery = "UPDATE Eigenschappen SET Waarde=@wa WHERE Id=@id";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(updatequery, con);
                command.Parameters.AddWithValue("@wa", Waarde);
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
                mess.Bericht = "upd_succes";
            }
            return mess;
        }

        [WebMethod]
        public void DELETEWaarde(string Catid, string Eigenschap, string Waarde)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND Waarde=@wa";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(deletequery, con);
            cmd.Parameters.AddWithValue("@catid", Catid);
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            cmd.Parameters.AddWithValue("@wa", Waarde);
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
        public Message REMOVESiblings(string Catid, string Eigenschap)
        {
            Message mess = new Message();

            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND NOT Waarde=''";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(deletequery, con);
            cmd.Parameters.AddWithValue("@catid", Catid);
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
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
            mess.Bericht = "rem_succes";
            return mess;
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
