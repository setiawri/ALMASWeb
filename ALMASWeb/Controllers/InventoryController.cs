using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Newtonsoft.Json;
using ALMASWeb.Controllers;
using ALMASWeb.Models;
using LIBUtil;

namespace ALMASWeb.Controllers
{
    public class InventoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Inventory
        public ActionResult Index(int? rss, int? InventoryGroup, string InventoryCategory, string InventoryType, int? Warehouse)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

			InventoryGroupController.setDropDownListViewBag(db, this, OperatorController.getUsername(Session));
			WarehouseController.setDropDownListViewBag(db, this, OperatorController.getUsername(Session));

			List<InventoryModel> models = null;
            if (InventoryGroup != null)
				models = get(null, InventoryGroup, InventoryCategory, InventoryType, Warehouse, OperatorController.getUsername(Session));

			return View(models);
        }

        /* METHODS ********************************************************************************************************************************************/

        private List<InventoryModel> get(Guid? id, int? GroupID, string CategoryID, string InventoryType, int? WarehouseID, string UserName)
        {
            List<InventoryModel> models = db.Database.SqlQuery<InventoryModel>(@"
					SELECT Inventory.*,
						InventoryGroup.Code,
						InventoryGroup.Name,
						InventoryGroup.CategoryName,
						InventoryGroup.TypeName,
						InventoryGroup.HasComponent,
						InventoryGroup.HasSpecification,
										  ISNULL(WarehouseStock.Stock,0) AS Stock,
						CASE
							WHEN Inventory.ThirdRatio > 0 THEN 
								CONVERT(VARCHAR(MAX), FLOOR(ISNULL(WarehouseStock.Stock, 0) / Inventory.ThirdRatio)) 
								+ ' ' + Unit3.Unit
								+ (CASE
										WHEN ISNULL(WarehouseStock.Stock,0) % Inventory.ThirdRatio > 0 THEN 
											' ' + CONVERT(VARCHAR(MAX), FLOOR((ISNULL(WarehouseStock.Stock,0) % Inventory.ThirdRatio) / Inventory.SecRatio))
											+ ' ' + Unit2.Unit
											+ (CASE
													WHEN FLOOR((ISNULL(WarehouseStock.Stock,0) % Inventory.ThirdRatio) % Inventory.SecRatio) > 0 THEN 
														' ' + CONVERT(VARCHAR(MAX), FLOOR((ISNULL(WarehouseStock.Stock,0) % Inventory.ThirdRatio) % Inventory.SecRatio))
														+ ' ' + Unit1.Unit
													ELSE ''
												END)
										ELSE ''
									END)
							WHEN Inventory.SecRatio > 0 THEN 
								CONVERT(VARCHAR(MAX), (ISNULL(WarehouseStock.Stock, 0) / Inventory.SecRatio)) 
								+ ' ' + Unit2.Unit
								+ (CASE
										WHEN (ISNULL(WarehouseStock.Stock,0) % Inventory.SecRatio) > 0 THEN 									
											CONVERT(VARCHAR(MAX), (ISNULL(WarehouseStock.Stock,0) % Inventory.SecRatio))
											+ ' ' + Unit1.Unit
										ELSE ''
									END)
							ELSE CONVERT(VARCHAR(MAX), ISNULL(WarehouseStock.Stock, 0)) + ' ' + Unit1.Unit
						END AS FormattedStock,
										  CASE
							WHEN Inventory.SecRatio > 0 THEN CONVERT(VARCHAR(MAX), FORMAT(Inventory.SecRatio,'N0')) + '/' + Unit2.Unit
							ELSE ''
						END AS Unit2Info,
										  CASE
							WHEN Inventory.ThirdRatio > 0 THEN CONVERT(VARCHAR(MAX), FORMAT(Inventory.ThirdRatio,'N0')) + '/' + Unit3.Unit
							ELSE ''
										  END AS Unit3Info,
						Unit1.Unit AS Unit1,
						Unit2.Unit AS Unit2,
						Unit3.Unit AS Unit3
									  FROM DWSystem.Inventory
										  LEFT JOIN DWSystem.InventoryGroup ON InventoryGroup.GroupID = Inventory.GroupID
						LEFT JOIN DWSystem.InventoryCategory ON InventoryCategory.CategoryID = Inventory.CategoryID
						LEFT JOIN DWSystem.InventoryUnit Unit1 ON Unit1.UnitID = Inventory.UnitID
						LEFT JOIN DWSystem.InventoryUnit Unit2 ON Unit2.UnitID = Inventory.SecUnitID
						LEFT JOIN DWSystem.InventoryUnit Unit3 ON Unit3.UnitID = Inventory.ThirdUnitID
						LEFT JOIN (
								SELECT WarehouseStock.InventoryID, ISNULL(SUM(WarehouseStock.Stock),0) AS Stock
								FROM dwsystem.WarehouseStock
									left join dwsystem.Warehouse on Warehouse.WarehouseID = WarehouseStock.WarehouseID
									left join DWSystem.Inventory on Inventory.InventoryID = WarehouseStock.InventoryID
									LEFT JOIN DWSystem.WarehouseAccess ON WarehouseAccess.WarehouseID = Warehouse.WarehouseID
								WHERE 1=1
									AND (@WarehouseID IS NULL OR Warehouse.WarehouseID = @WarehouseID)
									AND (@UserName IS NULL OR WarehouseAccess.UserName = @UserName)
								GROUP BY WarehouseStock.InventoryID
							) WarehouseStock ON WarehouseStock.InventoryID = Inventory.InventoryID
									  WHERE 1=1
						AND (@GroupID IS NULL OR Inventory.GroupID = @GroupID)
						AND (@CategoryID IS NULL OR Inventory.CategoryID = @CategoryID)
						AND (@CategoryID IS NOT NULL OR (
							@UserName IS NOT NULL 
							AND Inventory.CategoryID IN (								
								SELECT InventoryCategory.CategoryID
								FROM DWSystem.InventoryCategory
								WHERE 
									InventoryCategory.GroupID IN (										
										SELECT InventoryGroup.GroupID
										FROM DWSystem.InventoryGroup
											LEFT JOIN DWSystem.InventoryGroupAccess ON InventoryGroupAccess.GroupID = InventoryGroup.GroupID
										WHERE InventoryGroupAccess.UserName = @UserName
									)
									AND InventoryCategory.CategoryID IN (								
										SELECT Inventory.CategoryID
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
						))
						AND (@TypeID IS NULL OR Inventory.TypeID = @TypeID)
						AND (@TypeID IS NOT NULL OR (
							@UserName IS NOT NULL 
							AND  Inventory.TypeID IN (
								SELECT InventoryType.TypeID
								FROM DWSystem.InventoryType
								WHERE 
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
						))
					ORDER BY Inventory.InventoryID
                    ",
                    DBConnection.getSqlParameter(InventoryModel.COL_InventoryID.Name, id),
                    DBConnection.getSqlParameter(InventoryModel.COL_GroupID.Name, GroupID),
                    DBConnection.getSqlParameter(InventoryModel.COL_CategoryID.Name, CategoryID),
                    DBConnection.getSqlParameter(InventoryModel.COL_TypeID.Name, InventoryType),
                    DBConnection.getSqlParameter(InventoryModel.COL_WarehouseID.Name, WarehouseID),
					DBConnection.getSqlParameter("UserName", UserName)
				).ToList();

			return models;
        }

        public JsonResult getInventoryCategoryList(int? GroupID)
        {
            List<InventoryCategoryModel> models = InventoryCategoryController.get(db, OperatorController.getUsername(Session), GroupID);

            return Json(new { models }, JsonRequestBehavior.AllowGet);
        }

        /******************************************************************************************************************************************************/
    }
}