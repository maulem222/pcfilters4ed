using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for ImageUploadHandler
    /// </summary>
    public class ImageUploadHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        // Generate a unique name for the image
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = context.Server.MapPath("~/odimages/" + uniqueFileName);

                        // Save the file
                        file.SaveAs(filePath);

                        // Return the URL of the saved image
                        string imageUrl = "/odimages/" + uniqueFileName;
                        context.Response.Write("{\"url\": \"" + imageUrl + "\"}");
                    }
                    catch (Exception ex)
                    {
                        context.Response.Write("{\"error\": \"Error saving file: " + ex.Message + "\"}");
                    }
                }
                else
                {
                    context.Response.Write("{\"error\": \"No file uploaded.\"}");
                }
            }
            else
            {
                context.Response.Write("{\"error\": \"No files received.\"}");
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