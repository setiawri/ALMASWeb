using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ALMASWeb.Controllers;
using LIBUtil;

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

        public const string APP_VERSION = "v201114";
        public const string COMPANYNAME = "PT. ALMAS";

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
        public static void setFilterViewBag(ControllerBase controller, int? InventoryGroup, string InventoryCategory, string InventoryType, int? Warehouse, string search)
        {
            var ViewBag = controller.ViewBag;
            ViewBag.Filter_InventoryGroup = Util.validateParameter(InventoryGroup);
            ViewBag.Filter_InventoryCategory = Util.validateParameter(InventoryCategory);
            ViewBag.Filter_InventoryType = Util.validateParameter(InventoryType);
            ViewBag.Filter_Warehouse = Util.validateParameter(Warehouse);
            ViewBag.Filter_Search = Util.validateParameter(search);
        }

        /******************************************************************************************************************************************************/
    }
}