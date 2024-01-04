using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LIBUtil;
using LIBWebMVC;

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
        public static bool isPTAlmas = false;

        public static string COMPANYNAME = isPTAlmas ? "PT. ALMAS" : "CV. ALMAS";

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
        public static void setFilterViewBag(ControllerBase controller, int? InventoryGroup, string InventoryCategory, string InventoryType, int? Warehouse, bool? chkOnlyHasStock, string search)
        {
            var ViewBag = controller.ViewBag;
            ViewBag.Filter_InventoryGroup = UtilWebMVC.validateParameter(InventoryGroup);
            ViewBag.Filter_InventoryCategory = UtilWebMVC.validateParameter(InventoryCategory);
            ViewBag.Filter_InventoryType = UtilWebMVC.validateParameter(InventoryType);
            ViewBag.Filter_Warehouse = UtilWebMVC.validateParameter(Warehouse);
            ViewBag.FILTER_OnlyHasStock = UtilWebMVC.validateParameter(chkOnlyHasStock);
            ViewBag.Filter_Search = UtilWebMVC.validateParameter(search);
        }

        /******************************************************************************************************************************************************/
    }
}