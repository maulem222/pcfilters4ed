using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for CategorieService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CategorieService : System.Web.Services.WebService
    {

        public class Child
        {
            public string Id;
            public string Categorienaam;
            public string Overzichtsafb;
        }

        public class Parent
        {
            public string Id;
            public string Categorienaam;
        }

        public class Category
        {
            public string id;
            public string catnaam;
            public string parentid;
        }

        static List<Child> categorielijst = new List<Child> { };

        [WebMethod]
        public string GetCategoriesSorted()
        {
            string query = "SELECT * FROM Crosscat ORDER BY Id";
            SqlCommand cmd = new SqlCommand(query);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        public class Message
        {
            public string Bericht;
        }

        [WebMethod]
        public Message hasSiblings(string id)
        {
            string query = "SELECT * FROM Crosscat WHERE ParentId=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            Message mess = new Message();

            if (dt.Rows.Count > 0)
            {
                mess.Bericht = "siblings";
            }
            else
            {
                mess.Bericht = "no_siblings";
            }
            return mess;
        }

        [WebMethod]
        public Category GetCrumbforPath(string id)
        {
            string query = "SELECT * FROM Crosscat WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            Category cat = new Category();
            foreach (DataRow row in dt.Rows)
            {
                cat.id = row["Id"].ToString();
                cat.parentid = row["ParentId"].ToString();
                cat.catnaam = row["Catnaam"].ToString();
            }
            return cat;
        }

        [WebMethod]
        public List<Child> GetCategories(string parentid)
        {
            categorielijst.Clear();
            string query = "SELECT * FROM Crosscat WHERE ParentId=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(parentid));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Child ch = new Child();
                ch.Id = row["Id"].ToString();
                ch.Categorienaam = row["Catnaam"].ToString();
                categorielijst.Add(ch);
            }
            return categorielijst;
        }

        [WebMethod]
        public Parent GetCrumb(string id)
        {
            string query = "SELECT * FROM Crosscat WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            Parent pa = new Parent();
            foreach (DataRow row in dt.Rows)
            {
                pa.Id = row["Id"].ToString();
                pa.Categorienaam = row["Catnaam"].ToString();
            }
            return pa;
        }

        public class Cat
        {
            public string Id;
        }

        static List<Cat> catlijst = new List<Cat> { };

        [WebMethod]
        public List<Cat> SelectCatsWithParent(string id)
        {
            catlijst.Clear();
            string query = "SELECT Id FROM Crosscat WHERE ParentId=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Cat cat = new Cat();
                cat.Id = row["Id"].ToString();
                catlijst.Add(cat);
            }
            return catlijst;
        }

        [WebMethod]
        public Child getCategoriegeg(string childid)
        {
            string query = "SELECT * FROM Crosscat WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(childid));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            Child ch = new Child();
            foreach (DataRow row in dt.Rows)
            {
                ch.Id = row["Id"].ToString();
                ch.Categorienaam = row["Catnaam"].ToString();
                ch.Overzichtsafb = row["Overzichtsafb"].ToString();
            }
            return ch;
        }

        [WebMethod]
        public void DeleteCatWithId(string id)
        {
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string deletequery = "DELETE FROM Crosscat WHERE Id=@id";
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
        public void INSERTCat(string nieuwecategorie, string parentid)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string insertquery = "INSERT INTO Crosscat (Catnaam, ParentId) VALUES (@catnaam, @parid)";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(insertquery, con);
            command.Parameters.AddWithValue("@catnaam", nieuwecategorie);
            command.Parameters.AddWithValue("@parid", parentid);
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
        public void UPDATECat(string gewijzigdecategorie, string overzichtsafbeelding, string childid)
        {
            string con_str = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            string updatequery = "UPDATE Crosscat SET Catnaam=@gewcat, Overzichtsafb=@afb WHERE Id=@id";
            SqlConnection con = new SqlConnection(con_str);
            SqlCommand command = new SqlCommand(updatequery, con);
            command.Parameters.AddWithValue("@gewcat", gewijzigdecategorie);
            command.Parameters.AddWithValue("@afb", overzichtsafbeelding);
            command.Parameters.AddWithValue("@id", Convert.ToInt32(childid));
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
