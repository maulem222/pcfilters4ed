using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for FileService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class FileService : System.Web.Services.WebService
    {

        [WebMethod]
        public void DeleteFile(String destfile)
        {
            if (System.IO.File.Exists(destfile))
            {
                // Use a try block to catch IOExceptions, to 
                // handle the case of the file already being 
                // opened by another process. 
                try
                {
                    System.IO.File.Delete(destfile);
                }
                catch (System.IO.IOException)
                {
                    return;
                }
            }
        }

        public class Message
        {
            public string sourceexists;
            public string destexists;
        }

        [WebMethod]
        public Message Check(String sourcefile, String destfile)
        {
            var message = new Message();
            String sourceexists = "false";
            String destexists = "false";

            string source = HttpContext.Current.Server.MapPath(sourcefile);
            string dest = HttpContext.Current.Server.MapPath(destfile);
            if (File.Exists(source))
            {
                sourceexists = "true";
            }
            //
            if (File.Exists(dest))
            {
                destexists = "true";
            }
            message.sourceexists = sourceexists;
            message.destexists = destexists;
            return message;
        }

        [WebMethod]
        public void Copy_eid(String sourcefile, String destfile, String catid, String html, String slidercode, String tablename)
        {
            string source = HttpContext.Current.Server.MapPath(sourcefile);
            string dest = HttpContext.Current.Server.MapPath(destfile);

            try
            {
                File.Copy(source, dest, true);
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }


            var text = new StringBuilder();

            foreach (string s in File.ReadAllLines(dest))
            {
                text.AppendLine(s.Replace("##CATID##", catid).Replace("##HTML##", html).Replace("##SLIDERCODE##", slidercode).Replace("##TABLE##", tablename));
            }

            using (var file = new StreamWriter(File.Create(dest)))
            {
                file.Write(text.ToString());
            }
        }

        [WebMethod]
        public void Copy_od(String sourcefile, String destfile, String catid)
        {
            string source = HttpContext.Current.Server.MapPath(sourcefile);
            string dest = HttpContext.Current.Server.MapPath(destfile);

            try
            {
                File.Copy(source, dest, true);
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }


            var text = new StringBuilder();

            foreach (string s in File.ReadAllLines(dest))
            {
                text.AppendLine(s.Replace("##CATID##", catid));
            }

            using (var file = new StreamWriter(File.Create(dest)))
            {
                file.Write(text.ToString());
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateFolder(string path)
        {
            try
            {
                string physicalPath = HttpContext.Current.Server.MapPath(path);
                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                    return $"Folder created: {path}";
                }
                else
                {
                    return $"Folder already exists: {path}";
                }
            }
            catch (Exception ex)
            {
                return $"Error creating folder: {ex.Message}";
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateFile(string path)
        {
            try
            {
                string physicalPath = HttpContext.Current.Server.MapPath(path);
                string directory = Path.GetDirectoryName(physicalPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(physicalPath))
                {
                    File.WriteAllText(physicalPath, string.Empty);
                    return $"File created: {path}";
                }
                else
                {
                    return $"File already exists: {path}";
                }
            }
            catch (Exception ex)
            {
                return $"Error creating file: {ex.Message}";
            }
        }
    }
}
