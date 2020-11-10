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

namespace ALMASWeb.Areas.PAYROLL.Controllers
{
    public class InventoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/Inventory
        public ActionResult Index(int? rss, int? InventoryGroup, string InventoryCategory, int? Warehouse)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            InventoryGroupController.setDropDownListViewBag(db, this, OperatorController.getUsername(Session));
            WarehouseController.setDropDownListViewBag(db, this, OperatorController.getUsername(Session));
            InventoryTypeController.setDropDownListViewBag(db, this, OperatorController.getUsername(Session));

            List<InventoryModel> models = null;
            if (InventoryGroup != null)
                models = get(null, InventoryGroup, InventoryCategory, Warehouse);

            return View(models);
        }

        /* METHODS ********************************************************************************************************************************************/

        private List<InventoryModel> get(Guid? id, int? GroupID, string CategoryID, int? WarehouseID)
        {
            List<InventoryModel> models = db.Database.SqlQuery<InventoryModel>(@"
                        SELECT Inventory.*,
							InventoryGroup.Code,
							InventoryGroup.Name,
							InventoryGroup.CategoryName,
							InventoryGroup.TypeName,
							InventoryGroup.HasComponent,
							InventoryGroup.HasSpecification,
                            Warehouse.WarehouseID
                        FROM DWSystem.Inventory
                            LEFT JOIN DWSystem.InventoryGroup ON InventoryGroup.GroupID = Inventory.GroupID
							LEFT JOIN DWSystem.InventoryCategory ON InventoryCategory.CategoryID = Inventory.CategoryID
							LEFT JOIN DWSystem.WarehouseStock ON WarehouseStock.InventoryID = Inventory.InventoryID
							LEFT JOIN DWSystem.Warehouse ON Warehouse.WarehouseID = WarehouseStock.WarehouseID
                        WHERE 1=1
							AND (@GroupID IS NULL OR Inventory.GroupID = @GroupID)
							AND (@GroupID IS NOT NULL OR (1=1
								AND (@CategoryID IS NULL OR Inventory.CategoryID = @CategoryID)
								AND (@WarehouseID IS NULL OR Warehouse.WarehouseID = @WarehouseID)
							))
                    ",
                    DBConnection.getSqlParameter(InventoryModel.COL_InventoryID.Name, id),
                    DBConnection.getSqlParameter(InventoryModel.COL_GroupID.Name, GroupID),
                    DBConnection.getSqlParameter(InventoryModel.COL_CategoryID.Name, CategoryID),
                    DBConnection.getSqlParameter(InventoryModel.COL_WarehouseID.Name, WarehouseID)
                ).ToList();

            foreach (InventoryModel model in models)
            {
            }

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