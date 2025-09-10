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
    /// Summary description for EigenschappenService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class EigenschappenService : System.Web.Services.WebService
    {

        public class Eigsch
        {
            public string Id;
            public string Eigenschap;
            public string FilterElement;
        }

        static List<Eigsch> eigenschappenlijst = new List<Eigsch> { };

        [WebMethod]
        public List<Eigsch> getEigenschappen(string Catid)
        {
            eigenschappenlijst.Clear();
            string query = "SELECT * FROM Eigenschappen WHERE Catid=@catid AND Waarde=@waarde";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@waarde", "");
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Eigsch eig = new Eigsch();
                eig.Id = row["Id"].ToString();
                eig.Eigenschap = row["Eigenschap"].ToString();
                eig.FilterElement = row["FilterElement"].ToString();
                eigenschappenlijst.Add(eig);
            }
            return eigenschappenlijst;
        }

        public class Eigensch
        {
            public string Id;
            public string Eigenschap;
            public string FilterElement;
            public string Min;
            public string Max;
            public string Step;
            public string Symbool;
            public string StatDyn;
        }

        [WebMethod]
        public Eigensch GetEigGegevens(string EigId)
        {
            string query = "SELECT * FROM Eigenschappen WHERE Id=@eigid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@eigid", Convert.ToInt32(EigId));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];

            Eigensch eigensch = new Eigensch();
            foreach (DataRow row in dt.Rows)
            {
                eigensch.Id = row["Id"].ToString();
                eigensch.Eigenschap = row["Eigenschap"].ToString();
                eigensch.FilterElement = row["FilterElement"].ToString();
                eigensch.Min = row["Min"].ToString();
                eigensch.Max = row["Max"].ToString(); 
                eigensch.Step = row["Step"].ToString();
                eigensch.Symbool = row["Symbool"].ToString();
                eigensch.StatDyn = row["StatDyn"].ToString();
            }
            return eigensch;
        }

        public class Message
        {
            public string Bericht;
        }

        [WebMethod]
        public Message INSERTEig(string Eigenschap, string Catid, string FilterElement, string Min, string Max, string Step, string Symbool, string StatDyn)
        {
            string query = "SELECT * FROM Eigenschappen WHERE Eigenschap=@eig and Catid=@catid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            cmd.Parameters.AddWithValue("@catid", Catid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            // doe dit alleen indien eigenschap niet al bestaat
            // retourneer naar gelang inv_succes of eig_bestaat string

            Message mess = new Message();

            if (dt.Rows.Count > 0)
            {
                mess.Bericht = "eig_bestaat";
            }
            else // Eigenschap bestaat nog niet
            {
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string insertquery = "INSERT INTO Eigenschappen (Eigenschap, Waarde, Catid, FilterElement, Min, Max, Step, Symbool, StatDyn) VALUES (@eig, @wa, @catid, @elem, @min, @max, @step, @symb, @st)";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(insertquery, con);
                command.Parameters.AddWithValue("@eig", Eigenschap);
                command.Parameters.AddWithValue("@wa", "");
                command.Parameters.AddWithValue("@catid", Catid);
                command.Parameters.AddWithValue("@elem", FilterElement);
                command.Parameters.AddWithValue("@min", Min);
                command.Parameters.AddWithValue("@max", Max);
                command.Parameters.AddWithValue("@step", Step);
                command.Parameters.AddWithValue("@symb", Symbool);
                command.Parameters.AddWithValue("@st", StatDyn);
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
        public Message UPDATEEig(string Id, string Eigenschap, string Catid, string FilterElement, string Min, string Max, string Step, string Symbool, string StatDyn)
        {
            Message mess = new Message();

            string Eigenschap_fd = "";

            string query = "SELECT Eigenschap FROM Eigenschappen WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Id);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Eigenschap_fd = row["Eigenschap"].ToString();
            }

            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string updatequery = "UPDATE Eigenschappen SET Eigenschap=@eig, FilterElement=@filt, Min=@min, Max=@max, Step=@step, Symbool=@symb, StatDyn=@st WHERE Eigenschap=@eig_fd AND Catid=@catid";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(updatequery, con);
            command.Parameters.AddWithValue("@eig", Eigenschap);
            command.Parameters.AddWithValue("@filt", FilterElement);
            command.Parameters.AddWithValue("@min", Min);
            command.Parameters.AddWithValue("@max", Max);
            command.Parameters.AddWithValue("@step", Step);
            command.Parameters.AddWithValue("@symb", Symbool);
            command.Parameters.AddWithValue("@st", StatDyn);
            command.Parameters.AddWithValue("@eig_fd", Eigenschap_fd);
            command.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
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
            return mess;
        }

        [WebMethod]
        public void DELETEEig(string Catid, string Eigenschap)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig";
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
        }

        [WebMethod]
        public void DeleteEigenschappenWithId(string Catid)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Eigenschappen WHERE Catid=@catid";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(deletequery, con);
            cmd.Parameters.AddWithValue("@catid", Catid);
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
