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

        public static List<InventoryTypeModel> get(DBContext db, string UserName, int? GroupID)
        {
            return db.Database.SqlQuery<InventoryTypeModel>(@"
					SELECT InventoryType.*
					FROM DWSystem.InventoryType
					WHERE 1=1
						AND (@UserName IS NULL 
							OR (
								InventoryType.GroupID IN (										
									SELECT InventoryGroup.GroupID
									FROM DWSystem.InventoryGroup
										LEFT JOIN DWSystem.InventoryGroupAccess ON InventoryGroupAccess.GroupID = InventoryGroup.GroupID
									WHERE InventoryGroupAccess.UserName = @UserName
								)
								AND InventoryType.TypeID IN (								
									SELECT Inventory.TypeID
									FROM DWSystem.Inventory
									WHERE Inventory.InventoryID IN (						
										SELECT WarehouseStock.InventoryID
										FROM DWSystem.WarehouseStock
										WHERE WarehouseStock.WarehouseID IN (						
											SELECT Warehouse.WarehouseID
											FROM DWSystem.Warehouse
												LEFT JOIN DWSystem.WarehouseAccess ON WarehouseAccess.WarehouseID = Warehouse.WarehouseID
											WHERE WarehouseAccess.UserName = @UserName
										)
									)										
								)
							)
						)
						AND (@GroupID IS NULL OR InventoryType.GroupID = @GroupID)
					ORDER BY InventoryType.Name ASC
                    ",
                    DBConnection.getSqlParameter("UserName", UserName),
					DBConnection.getSqlParameter(InventoryTypeModel.COL_GroupID.Name, GroupID)
				).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}