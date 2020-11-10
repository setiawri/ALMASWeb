using ALMASWeb.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using System.Collections.Generic;

namespace ALMASWeb.Controllers
{
    public class InventoryTypeController : Controller
    {
        private readonly DBContext db = new DBContext();

        public static List<InventoryTypeModel> get(DBContext db, string UserName)
        {
            return db.Database.SqlQuery<InventoryTypeModel>(@"
					SELECT InventoryType.*
					FROM DWSystem.InventoryType
					WHERE InventoryType.TypeID IN (					
						SELECT Inventory.TypeID
						FROM DWSystem.Inventory
						WHERE Inventory.InventoryID IN (						
								SELECT WarehouseStock.InventoryID
								FROM DWSystem.WarehouseStock
								WHERE WarehouseStock.WarehouseID IN (						
										SELECT Warehouse.WarehouseID
										FROM DWSystem.Warehouse
											LEFT JOIN DWSystem.WarehouseAccess ON WarehouseAccess.WarehouseID = Warehouse.WarehouseID
										WHERE 1=1
											AND (@UserName IS NULL OR WarehouseAccess.UserName = @UserName)
									)
							)
					)
					ORDER BY InventoryType.Name ASC
                    ",
                    DBConnection.getSqlParameter(WarehouseAccessModel.COL_UserName.Name, UserName)
                ).ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller, string UserName)
        {
            List<InventoryTypeModel> models = get(db, UserName);
            controller.ViewBag.InventoryType = new SelectList(models,
                    InventoryTypeModel.COL_TypeID.Name, InventoryTypeModel.COL_Name.Name);
            controller.ViewBag.InventoryTypeCount = models.Count;
        }

        /******************************************************************************************************************************************************/
    }
}