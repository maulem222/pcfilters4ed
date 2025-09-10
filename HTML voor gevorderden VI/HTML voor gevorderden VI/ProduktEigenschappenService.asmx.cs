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
    /// Summary description for ProduktEigenschappenService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ProduktEigenschappenService : System.Web.Services.WebService
    {
        public class EigenschapsWaarde
        {
            public string Id;
            public string Waarde;
            public string Voorkomen;
        }

        static List<EigenschapsWaarde> eigenschapswaardenlijst = new List<EigenschapsWaarde> { };

        [WebMethod]
        public List<EigenschapsWaarde> CheckVoorkomenEigIds(string Produktid, string[] ArrayVanEigIds)
        {
            string Id;
            string EigenschapsWaarde;

            eigenschapswaardenlijst.Clear();

            foreach (var eigid in ArrayVanEigIds)
            {
                // haal het record uit produkteigenschappen op
                string query = "SELECT * FROM ProduktEigenschappen WHERE ProduktId=@prodid AND EigenschapsId=@eigid";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@prodid", Produktid);
                cmd.Parameters.AddWithValue("@eigid", Convert.ToInt32(eigid));
                DataSet ds = GetData(cmd);
                DataTable dt = ds.Tables[0];
                // haal de waarde van de eigenschap op
                string query2 = "SELECT Id, Waarde FROM Eigenschappen WHERE Id=@eigid";
                SqlCommand cmd2 = new SqlCommand(query2);
                cmd2.Parameters.AddWithValue("@eigid", Convert.ToInt32(eigid));
                DataSet ds2 = GetData(cmd2);
                DataTable dt2 = ds2.Tables[0];
                // en leg deze vast
                    foreach (DataRow row in dt2.Rows)
                    {
                        Id = row["Id"].ToString();
                        EigenschapsWaarde = row["Waarde"].ToString();
                        EigenschapsWaarde ew = new EigenschapsWaarde();

                        if (dt.Rows.Count > 0) // eigenschap komt voor bij dit produkt
                        {
                            ew.Id = Id;
                            ew.Waarde = EigenschapsWaarde;
                            ew.Voorkomen = "Ja";
                        }
                        else // eigenschap komt niet voor
                        {
                            ew.Id = Id;
                            ew.Waarde = EigenschapsWaarde;
                            ew.Voorkomen = "Nee";
                        }
                        eigenschapswaardenlijst.Add(ew);
                    }
            }
            return eigenschapswaardenlijst;
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
