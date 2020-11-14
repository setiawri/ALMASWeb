using ALMASWeb.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using System.Collections.Generic;

namespace ALMASWeb.Controllers
{
    public class InventoryGroupController : Controller
    {
        private readonly DBContext db = new DBContext();

        public static List<InventoryGroupModel> get(DBContext db, string UserName)
        {
            return db.Database.SqlQuery<InventoryGroupModel>(@"
                    SELECT InventoryGroup.*
                    FROM DWSystem.InventoryGroup
                    	LEFT JOIN DWSystem.InventoryGroupAccess ON InventoryGroupAccess.GroupID = InventoryGroup.GroupID
                    WHERE 1=1
                    	AND (@UserName IS NULL OR InventoryGroupAccess.UserName = @UserName)
                    ORDER BY InventoryGroup.GroupID ASC
                    ",
                    DBConnection.getSqlParameter(WarehouseAccessModel.COL_UserName.Name, UserName)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}