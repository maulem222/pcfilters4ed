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
    /// Summary description for FilterService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class FilterService : System.Web.Services.WebService
    {
        public class Eigsch
        {
            public string Id;
            public string Eigenschap;
            public string FilterElement;
        }

        static List<Eigsch> eigenschappenlijst = new List<Eigsch> { };

        [WebMethod]
        public List<Eigsch> GetEigenschappen(string Catid)
        {
            eigenschappenlijst.Clear();
            string query = "SELECT DISTINCT Eigenschap FROM Eigenschappen WHERE Catid=@catid";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Eigsch eig = new Eigsch();
                eig.Eigenschap = row["Eigenschap"].ToString();
                eigenschappenlijst.Add(eig);
            }
            return eigenschappenlijst;
        }

        public class Wa
        {
            public string Waarde;
        }

        public class IdenWa
        {
            public string Id;
            public string Waarde;
        }

        static List<Wa> waardenlijst = new List<Wa> { };

        static List<IdenWa> id_en_waardelijst = new List<IdenWa> { };

        [WebMethod]
        public List<Wa> GetWaarden(string Catid, string Eigenschap)
        {
            waardenlijst.Clear();
            string query = "SELECT Waarde FROM Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND NOT Waarde=''";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Wa waarde = new Wa();
                waarde.Waarde = row["Waarde"].ToString();
                waardenlijst.Add(waarde);
            }
            return waardenlijst;
        }

        [WebMethod]
        public List<IdenWa> GetIdEnWaarde(string Catid, string Eigenschap)
        {
            id_en_waardelijst.Clear();
            string query = "SELECT * FROM Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND NOT Waarde=''";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                IdenWa idwaarde = new IdenWa();
                idwaarde.Id = row["Id"].ToString();
                idwaarde.Waarde = row["Waarde"].ToString();
                id_en_waardelijst.Add(idwaarde);
            }
            return id_en_waardelijst;
        }

        public class Filter
        {
            public string FilterElement;
        }

        [WebMethod]
        public Filter GetFilterElement(string Catid, string Eigenschap)
        {
            Filter filter = new Filter();
            string query = "SELECT FilterElement from Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND Waarde=''";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                filter.FilterElement = row["FilterElement"].ToString();
            }
            return filter;
        }

        public class Slider
        {
            public string Id;
            public string Min;
            public string Max;
            public string Step;
            public string Symbool;
            public string StatDyn;
        }

        [WebMethod]
        public Slider GetSlider(string Catid, string Eigenschap, string FilterElement)
        {
            Slider slider = new Slider();
            string query = "SELECT * from Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND Waarde='' AND FilterElement=@filt";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", Eigenschap);
            cmd.Parameters.AddWithValue("@filt", FilterElement);
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                slider.Id = row["Id"].ToString();
                slider.Min = row["Min"].ToString();
                slider.Max = row["Max"].ToString();
                slider.Step = row["Step"].ToString();
                slider.Symbool = row["Symbool"].ToString();
                slider.StatDyn = row["StatDyn"].ToString();
            }
            return slider;
        }

        public class PrijsSlider
        {
            public string Id;
            public string Waarde;
        }

        [WebMethod]
        public PrijsSlider GetSliderPrijs(string Catid)
        {
            PrijsSlider prijsslider = new PrijsSlider();
            string query = "SELECT * from Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND NOT Waarde=''";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", "Prijs");
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                prijsslider.Id = row["Id"].ToString();
                prijsslider.Waarde = row["Waarde"].ToString();
            }
            return prijsslider;
        }

        public PrijsSlider GetSliderPrijsId(string Catid)
        {
            PrijsSlider prijsslider = new PrijsSlider();
            string query = "SELECT * from Eigenschappen WHERE Catid=@catid AND Eigenschap=@eig AND Waarde=''";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@catid", Convert.ToInt32(Catid));
            cmd.Parameters.AddWithValue("@eig", "Prijs");
            DataSet ds = GetData(cmd);
            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                prijsslider.Id = row["Id"].ToString();
            }
            return prijsslider;
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
