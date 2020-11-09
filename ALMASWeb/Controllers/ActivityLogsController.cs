using ALMASWeb.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALMASWeb.Controllers
{
    public class ActivityLogsController : Controller
    {

        private readonly DBContext db = new DBContext();

        /* DISPLAY LOG ****************************************************************************************************************************************/

        public JsonResult GetLog(Guid ReffId)
        {
            string message = @"<div class='table-responsive'>
                                    <table class='table table-striped table-bordered'>
                                        <thead>
                                            <tr>
                                                <th>Timestamp</th>
                                                <th>Description</th>
                                                <th>Operator</th>
                                            </tr>
                                        </thead>
                                        <tbody>";

            var items = (from ActivityLogsModel in db.ActivityLogsModel
                               join OperatorModel in db.OperatorModel on ActivityLogsModel.Operator_ID equals OperatorModel.ID
                               where ActivityLogsModel.ReffId == ReffId
                               orderby ActivityLogsModel.Timestamp descending
                               select new { ActivityLogsModel, OperatorModel }).ToList();

            foreach (var item in items)
            {
                message += @"<tr>
                            <td>" + string.Format("{0:dd/MM/yyyy HH:mm}", item.ActivityLogsModel.Timestamp) + @"</td>
                            <td>" + item.ActivityLogsModel.Description + @"</td>
                            <td>" + item.OperatorModel.Name + @"</td>
                        </tr>";
            }

            message += "</tbody></table></div>";
            return Json(new { content = message }, JsonRequestBehavior.AllowGet);
        }

        /* ADD ************************************************************************************************************************************************/

        public static void AddEditLog(DBContext db, HttpSessionStateBase Session, Guid reffId, string log) { Add(db, Session, reffId, "UPDATE:<BR>" + log); }
        public static void AddCreateLog(DBContext db, HttpSessionStateBase Session, Guid reffId) { Add(db, Session, reffId, "Created"); }
        public static void Add(DBContext db, HttpSessionStateBase Session, Guid reffId, string description)
        {
            db.ActivityLogsModel.Add(new ActivityLogsModel
            {
                Id = Guid.NewGuid(),
                ReffId = reffId,
                Timestamp = DateTime.Now,
                Description = description,
                Operator_ID = OperatorController.getUserId(Session)
            });
        }

        public static string editStringFormat(string fieldName) { return fieldName + ": '{0}' to '{1}'"; }
        public static string editIntFormat(string fieldName) { return fieldName + ": '{0:N0}' to '{1:N0}'"; }
        public static string editDateFormat(string fieldName) { return fieldName + ": '{0:dd/MM/yyyy}' to '{1:dd/MM/yyyy}'"; }
        public static string editDateTimeFormat(string fieldName) { return fieldName + ": '{0:dd/MM/yyyy HH:mm}' to '{1:dd/MM/yyyy HH:mm}'"; }
        public static string editDecimalFormat(string fieldName, int decimalPoint) { return fieldName + ": '{0:N"+ decimalPoint + "}' to '{1:N" + decimalPoint + "}'"; }

        /******************************************************************************************************************************************************/
    }
}