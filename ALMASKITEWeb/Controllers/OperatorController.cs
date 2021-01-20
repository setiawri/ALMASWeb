using ALMASKITEWeb.Models;
using LIBUtil;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALMASKITEWeb.Controllers
{
    public class AccessList
    {
        public OperatorPrivilegeDataManagementModel OperatorPrivilegeDataManagement;

        public AccessList() { }

        public void populate(OperatorPrivilegeDataManagementModel operatorPrivilegeDataManagementModel)
        {
            OperatorPrivilegeDataManagement = operatorPrivilegeDataManagementModel;
        }
    }

    public class OperatorController : Controller
    {
        private readonly DBContext db = new DBContext();

        public const string LOGIN_ACTIONNAME = "Login";
        public const string LOGIN_CONTROLLERNAME = "Operator";
        public const string LOGIN_AREANAME = "";

        public const string SESSION_UserId = "UserId";
        public const string SESSION_Username = "Username";
        public const string SESSION_OperatorPrivilegeDataManagement_InventoryList = "OperatorPrivilegeDataManagement_InventoryList";

        /* LOGIN PAGE *****************************************************************************************************************************************/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(OperatorModel model, string returnUrl)
        {
            //bypass login
            if (true && Server.MachineName == "RQ-ASUS")
            {
                if (string.IsNullOrEmpty(model.UserName))
                    model.UserName = "TO";
                if (string.IsNullOrEmpty(model.Password))
                    model.Password = "admin";
            }

            string hashedPassword = HashPassword(model.Password);
            var result = (
                    from OperatorModel in db.OperatorModel
                    join OperatorPrivilegeDataManagementModel in db.OperatorPrivilegeDataManagementModel
                        on OperatorModel.UserName equals OperatorPrivilegeDataManagementModel.UserName
                    where OperatorModel.UserName.ToLower() == model.UserName.ToLower()
                        && OperatorModel.Password.ToLower() == hashedPassword.ToLower()
                    select new { OperatorModel, OperatorPrivilegeDataManagementModel }
                ).FirstOrDefault();

            if (result == null)
                ModelState.AddModelError("", "Invalid username or password");
            else
            {
                AccessList accessList = new AccessList();
                accessList.populate(result.OperatorPrivilegeDataManagementModel);

                setLoginSession(Session, result.OperatorModel.ID, result.OperatorModel.UserName, accessList);
                return RedirectToLocal(returnUrl);
            }

            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public ActionResult LogOff()
        {
            setLoginSession(Session, null, null, new AccessList());
            return RedirectToAction(nameof(Login));
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public static string HashPassword(string value)
        {
            //truncate if more than 10 digits
            if (value.Length > 10)
                value = value.Substring(0, 10);

            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(value, "SHA1");
        }

        public static int getUserId(HttpSessionStateBase Session)
        {
            return int.Parse(Session[SESSION_UserId].ToString());
        }

        public static string getUsername(HttpSessionStateBase Session)
        {
            return Session[SESSION_Username].ToString();
        }

        public static bool isLoggedIn(HttpSessionStateBase session)
        {
            return session[SESSION_UserId] != null;
        }

        private static void setLoginSession(HttpSessionStateBase Session, object userId, object username, AccessList accessList)
        {
            Session[SESSION_UserId] = userId == null ? null : userId.ToString();
            Session[SESSION_Username] = username == null ? null : username.ToString();

            if (Session[SESSION_UserId] != null)
            {
                Session[SESSION_OperatorPrivilegeDataManagement_InventoryList] = accessList.OperatorPrivilegeDataManagement.InventoryList;
            }
        }

        /******************************************************************************************************************************************************/
    }
}