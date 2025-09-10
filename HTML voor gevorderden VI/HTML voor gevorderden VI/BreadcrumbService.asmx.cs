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
    /// Summary description for BreadcrumbService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BreadcrumbService : System.Web.Services.WebService
    {

        [WebMethod]
        public void InsORUpdBreadcrumb(string Catid, string Breadcrumb)
        {
            string query = "SELECT * FROM Breadcrumbs WHERE Catid=@catid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Catid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                // update Breadcrumb
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string updatequery = "UPDATE Breadcrumbs SET Breadcrumb=@bc WHERE Catid=@catid";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(updatequery, con);
                command.Parameters.AddWithValue("@bc", Breadcrumb);
                command.Parameters.AddWithValue("@catid", Catid);
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
            else
            {
                // insert Breadcrumb
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string insertquery = "INSERT INTO Breadcrumbs (Breadcrumb, Catid) VALUES (@bc, @catid)";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(insertquery, con);
                command.Parameters.AddWithValue("@bc", Breadcrumb);
                command.Parameters.AddWithValue("@catid", Catid);

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

        public class Breadcrumb
        {
            public string Crumb;
        }

        [WebMethod]
        public Breadcrumb ReadBCfromDB(string Catid)
        {
            Breadcrumb bc = new Breadcrumb();

            string query = "SELECT * FROM Breadcrumbs WHERE Catid=@catid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Catid);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                bc.Crumb = row["Breadcrumb"].ToString();
            }
            return bc;
        }

        [WebMethod]
        public void DeleteBreadcrumbWithId(string Catid)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Breadcrumbs WHERE Catid=@catid";
            SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand(deletequery, con);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
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
