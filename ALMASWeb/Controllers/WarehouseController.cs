using ALMASWeb.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using System.Collections.Generic;

namespace ALMASWeb.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly DBContext db = new DBContext();

        public static List<WarehouseModel> get(DBContext db, string UserName)
        {
            return db.Database.SqlQuery<WarehouseModel>(@"
                    SELECT Warehouse.*
                                   FROM DWSystem.Warehouse
            	        LEFT JOIN DWSystem.WarehouseAccess ON WarehouseAccess.WarehouseID = Warehouse.WarehouseID
                                   WHERE 1=1
                                       AND (@UserName IS NULL OR WarehouseAccess.UserName = @UserName)
                    ORDER BY Warehouse.WarehouseName ASC
                    ",
                    DBConnection.getSqlParameter(WarehouseAccessModel.COL_UserName.Name, UserName)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}