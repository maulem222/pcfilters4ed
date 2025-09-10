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
    /// Summary description for HTMLService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class HTMLService : System.Web.Services.WebService
    {

        public class HTML
        {
            public string html;
        }

        [WebMethod]
        public HTML GetHTML(string od_id)
        {
            HTML hc = new HTML();

            string query = "SELECT * FROM HTML WHERE od_id=@od_id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@od_id", od_id);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                hc.html = row["html"].ToString();
            }
            return hc;
        }

        [WebMethod]
        public void InsORUpdHTML(string od_id, string html)
        {
            string query = "SELECT * FROM HTML WHERE od_id=@od_id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@od_id", od_id);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                // update HTML
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string updatequery = "UPDATE HTML SET html=@html WHERE od_id=@od_id";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(updatequery, con);
                command.Parameters.AddWithValue("@html", html);
                command.Parameters.AddWithValue("@od_id", od_id);
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
                // insert HTML
                string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
                string insertquery = "INSERT INTO HTML (html, od_id) VALUES (@html, @od_id)";
                SqlConnection con = new SqlConnection(con_str);
                SqlCommand command = new SqlCommand(insertquery, con);
                command.Parameters.AddWithValue("@html", html);
                command.Parameters.AddWithValue("@od_id", od_id);

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
