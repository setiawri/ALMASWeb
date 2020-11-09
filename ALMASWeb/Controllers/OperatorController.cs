using ALMASWeb.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALMASWeb.Controllers
{
    public class OperatorController : Controller
    {
        private readonly DBContext db = new DBContext();

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
            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.Password);
                var result = (
                        from OperatorModel in db.OperatorModel
                        join OperatorPrivilegePayrollModel in db.OperatorPrivilegePayrollModel on OperatorModel.UserName equals OperatorPrivilegePayrollModel.UserName
                        where OperatorModel.UserName.ToLower() == model.UserName.ToLower()
                            && OperatorModel.Password.ToLower() == hashedPassword.ToLower()
                            && OperatorPrivilegePayrollModel.PayrollModule == true
                        select new { OperatorModel, OperatorPrivilegePayrollModel }
                    ).FirstOrDefault();

                if (result == null)
                    ModelState.AddModelError("", "Invalid username or password.");
                else
                {
                    login(Session, result.OperatorModel.ID, result.OperatorModel.Name);
                    return RedirectToLocal(returnUrl);
                }
            }

            return View(model);
        }


        public bool RetrieveOperatorInventoryGroupAccess(string UserName)
        {
            return db.Database.SqlQuery<bool>(@"
					SELECT InventoryGroup.*,
						ISNULL(InventoryGroup.TypeID,0) as TypeID,
						ISNULL(InventoryGroupType.Type,'-') as Type,
						InventoryGroupAccess.*
                    FROM DWSystem.InventoryGroup
							LEFT JOIN DWSystem.InventoryGroupType ON InventoryGroupType.TypeID = InventoryGroup.TypeID
							LEFT JOIN DWSystem.InventoryGroupAccess ON InventoryGroupAccess.GroupID = InventoryGroup.GroupID
                    WHERE InventoryGroupAccess.UserName = @UserName
                    ORDER BY InventoryGroup.GroupID ASC
                    ",
                DBConnection.getSqlParameter("UserName", UserName)
                ).FirstOrDefault();
        }
        

        /* METHODS ********************************************************************************************************************************************/

        public ActionResult LogOff()
        {
            login(Session, null, null);
            return RedirectToAction("Login");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public static string HashPassword(string input)
        {
            var hash = new System.Security.Cryptography.SHA1Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static int getUserId(HttpSessionStateBase Session)
        {
            return int.Parse(Session[Helper.SESSION_UserId].ToString());
        }

        public static bool isLoggedIn(HttpSessionStateBase session)
        {
            return session[Helper.SESSION_UserId] != null;
        }

        private static void login(HttpSessionStateBase session, object userId, object username)
        {
            session[Helper.SESSION_UserId] = userId == null ? null : userId.ToString();
            session[Helper.SESSION_Username] = username == null ? null : userId.ToString();
        }

        public static void setApprovalPrivilegeListViewBag(DBContext db, ControllerBase controller, HttpSessionStateBase Session)
        {
            int userID = getUserId(Session);
            var privilegePayroll = (from Operator in db.OperatorModel
                                    join OperatorPrivilegePayroll in db.OperatorPrivilegePayrollModel on Operator.UserName equals OperatorPrivilegePayroll.UserName
                                    where Operator.ID == userID
                                    select new { Operator, OperatorPrivilegePayroll }).FirstOrDefault();
            controller.ViewBag.ApprovalPrivilege = privilegePayroll == null ? false : privilegePayroll.OperatorPrivilegePayroll.Approval;
        }

        /******************************************************************************************************************************************************/
    }
}