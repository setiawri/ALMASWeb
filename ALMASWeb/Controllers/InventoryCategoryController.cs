using ALMASWeb.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using System.Collections.Generic;

namespace ALMASWeb.Controllers
{
    public class InventoryCategoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        public static List<InventoryCategoryModel> get(DBContext db, string UserName, int? GroupID)
        {
            return db.Database.SqlQuery<InventoryCategoryModel>(@"
					SELECT InventoryCategory.*
					FROM DWSystem.InventoryCategory
					WHERE InventoryCategory.CategoryID IN (					
							SELECT Inventory.CategoryID
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
						AND (@GroupID IS NULL OR InventoryCategory.GroupID = @GroupID)
					ORDER BY InventoryCategory.Name ASC
                    ",
                    DBConnection.getSqlParameter(WarehouseAccessModel.COL_UserName.Name, UserName),
                    DBConnection.getSqlParameter(InventoryCategoryModel.COL_GroupID.Name, GroupID)
                ).ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller, string UserName, int? GroupID)
        {
            List<InventoryCategoryModel> models = get(db, UserName, GroupID);
            controller.ViewBag.InventoryCategory = new SelectList(models,
                    InventoryCategoryModel.COL_CategoryID.Name, InventoryCategoryModel.COL_Name.Name);
            controller.ViewBag.InventoryCategoryCount = models.Count;
        }

        /******************************************************************************************************************************************************/
    }
}