using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for SessionService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SessionService : System.Web.Services.WebService
    {
        public class Session_ID
        {
            public string ID;
            public string IDset;
        }

        [WebMethod(EnableSession = true)]
        public void SetSession_ID(string newID)
        {
            HttpContext.Current.Session["newID"] = newID;
        }

        [WebMethod(EnableSession = true)]
        public Session_ID GetSession_ID()
        {
            Session_ID Session_ID = new Session_ID();
            if (HttpContext.Current.Session["newID"] != null)
            {
                Session_ID.ID = HttpContext.Current.Session["newID"].ToString();
                Session_ID.IDset = "Ja";
            }
            else
            {
                Session_ID.IDset = "Nee";
            }
            return Session_ID;
        }

        public class Session_Catid
        {
            public string Catid;
            public string Catidset;
        }

        [WebMethod(EnableSession = true)]
        public void SetSession_Catid(string Catid)
        {
            HttpContext.Current.Session["Catid"] = Catid;
        }

        [WebMethod(EnableSession = true)]
        public Session_Catid GetSession_Catid()
        {
            Session_Catid Session_Catid = new Session_Catid();
            if (HttpContext.Current.Session["Catid"] != null)
            {
                Session_Catid.Catid = HttpContext.Current.Session["Catid"].ToString();
                Session_Catid.Catidset = "Ja";
            }
            else
            {
                Session_Catid.Catidset = "Nee";
            }
            return Session_Catid;
        }
    }
}
