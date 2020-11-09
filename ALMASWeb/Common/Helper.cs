using System;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ALMASWeb
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

        public const string APP_VERSION = "v201110";
        public const string COMPANYNAME = "PT.ALMAS";

        public const string SESSION_UserId = "UserId";
        public const string SESSION_Username = "Username";
        public const string LOGIN_ACTIONNAME = "Login";
        public const string LOGIN_CONTROLLERNAME = "Operator";
        public const string LOGIN_AREANAME = "";

        public const string IMAGEFOLDERURL = "/assets/img/";
        public const string IMAGEFOLDERPATH = "~"+ IMAGEFOLDERURL;
        public const string NOIMAGEFILE = "no-image.jpg";

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