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
    /// Summary description for TableService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class TableService : System.Web.Services.WebService
    {

        [WebMethod]
        public string getTabellen()
        {
            string query = "SELECT * FROM Tabellen ORDER BY Tabelnaam ASC";
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        [WebMethod]
        public void UPDATETabellen(string id, string catid, string pad)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string updatequery = "UPDATE Tabellen SET Catid=@catid, Pad=@pad WHERE Id=@id";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(updatequery, con);
            command.Parameters.AddWithValue("@catid", catid);
            command.Parameters.AddWithValue("@pad", pad);
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

        public class Tabel
        {
            public string id;
            public string tabelnaam;
            public string catid;
            public string pad;
        }

        [WebMethod]
        public Tabel getCatidandPath(string id)
        {
            string query = "SELECT * FROM Tabellen WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            Tabel tab = new Tabel();
            foreach (DataRow row in dt.Rows)
            {
                tab.id = row["Id"].ToString();
                tab.tabelnaam = row["Tabelnaam"].ToString();
                tab.catid = row["Catid"].ToString();
                tab.pad = row["Pad"].ToString();
            }
            return tab;
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
