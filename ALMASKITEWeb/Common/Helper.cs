using ALMASKITEWeb.Controllers;
using LIBUtil;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ALMASKITEWeb
{
    public struct ModelMember
    {
        public string Name;
        public Guid Id;
        public string Display;
    }

    public class Helper
    {
        private static readonly DBContext db = new DBContext();

        /* PUBLIC PROPERTIES **********************************************************************************************************************************/

        public const string APP_VERSION = "v240104";
        public const string COMPANYNAME = "PT. ALMAS";

        public const string IMAGEFOLDERURL = "/assets/img/";
        public const string IMAGEFOLDERPATH = "~" + IMAGEFOLDERURL;
        public const string NOIMAGEFILE = "no-image.jpg";

        public const string APPCONFIG_REPORTEXCELPASSWORD = "reportexcelpassword";

        /* METHODS ********************************************************************************************************************************************/

        public static string getImageUrl(string imageName, HttpRequestBase Request, HttpServerUtilityBase Server)
        {
            string filename = NOIMAGEFILE;
            if (!string.IsNullOrEmpty(imageName))
            {
                string dir = Server.MapPath(IMAGEFOLDERPATH);
                string path = Path.Combine(dir, imageName);
                if (File.Exists(path))
                    filename = imageName;
            }

            return (Request.ApplicationPath + IMAGEFOLDERURL + filename).Replace("//", "/");
        }

        /******************************************************************************************************************************************************/
    }
}