using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for MenuHandler
    /// </summary>
    public class MenuHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<Menu> listMenu = new List<Menu>();

            string sqlquery = "SELECT * FROM Crosscat";
            string constr = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;
            SqlConnection con = new SqlConnection(constr);
            SqlCommand command = new SqlCommand(sqlquery, con);
            con.Open();
            SqlDataReader sdr = command.ExecuteReader();

            while (sdr.Read())
            {
                Menu menu = new Menu();
                menu.id = Convert.ToInt32(sdr["Id"]);
                menu.href = "#";
                menu.text = sdr["Catnaam"].ToString();
                menu.ParentId = Convert.ToInt32(sdr["ParentId"]);
                listMenu.Add(menu);
            }
            List<Menu> menuTree = GetMenuTree(listMenu, 0);

            JavaScriptSerializer js = new JavaScriptSerializer();
            context.Response.Write(js.Serialize(menuTree));
        }

        List<Menu> ch;

        private List<Menu> GetMenuTree(List<Menu> children, int parentId)
        {
            ch = children.Where(x => x.ParentId == parentId).Select(x => new Menu()
            {
                id = x.id,
                href = x.href,
                text = x.text,
                ParentId = x.ParentId,
                children = GetMenuTree(children, x.id)
                
            }).ToList();
            if (!ch.Any())
            {
                return null;
            }
            else
            {
                return ch;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}