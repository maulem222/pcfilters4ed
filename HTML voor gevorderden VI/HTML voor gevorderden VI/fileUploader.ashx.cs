using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for fileUploader
    /// </summary>
    public class fileUploader : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                string str_image_ext = "";

                foreach (string s in context.Request.Files)
                {
                    HttpPostedFile file = context.Request.Files[s];

                    string fileName = file.FileName;
                    string fileExtension = file.ContentType;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileExtension = Path.GetExtension(fileName);
                        str_image_ext = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = HttpContext.Current.Server.MapPath("~/afbeeldingen/" + str_image_ext);
                        file.SaveAs(filePath);
                    }
                }
                context.Response.Write(str_image_ext);
            }
            catch (Exception ex)
            {
                throw ex;
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