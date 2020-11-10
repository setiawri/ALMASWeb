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
					WHERE InventoryGroup.GroupID IN (					
						    SELECT Inventory.GroupID
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
					ORDER BY InventoryGroup.GroupID ASC
                    ",
                    DBConnection.getSqlParameter(WarehouseAccessModel.COL_UserName.Name, UserName)
                ).ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller, string UserName)
        {
            List<InventoryGroupModel> models = get(db, UserName);
            controller.ViewBag.InventoryGroup = new SelectList(models,
                    InventoryGroupModel.COL_GroupID.Name, InventoryGroupModel.COL_Name.Name);
            controller.ViewBag.InventoryGroupCount = models.Count;

            InventoryCategoryController.setDropDownListViewBag(db, controller, UserName, models[0].GroupID);
        }

        /******************************************************************************************************************************************************/
    }
}