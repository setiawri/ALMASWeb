using ALMASWeb.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using System.Collections.Generic;

namespace ALMASWeb.Controllers
{
    public class WarehouseStockController : Controller
    {
        private readonly DBContext db = new DBContext();

        public static List<WarehouseStockModel> get(DBContext db, string InventoryID, string UserName)
        {
            return db.Database.SqlQuery<WarehouseStockModel>(@"
					SELECT WarehouseStock.*,
						Warehouse.WarehouseName
					FROM dwsystem.WarehouseStock
						left join dwsystem.Warehouse on Warehouse.WarehouseID = WarehouseStock.WarehouseID
						left join DWSystem.Inventory on Inventory.InventoryID = WarehouseStock.InventoryID
						LEFT JOIN DWSystem.WarehouseAccess ON WarehouseAccess.WarehouseID = Warehouse.WarehouseID
					WHERE 1=1
						AND (@InventoryID IS NULL OR WarehouseStock.InventoryID = @InventoryID)
						AND (@UserName IS NULL OR WarehouseAccess.UserName = @UserName)
					ORDER BY Warehouse.WarehouseName ASC
                    ",
                    DBConnection.getSqlParameter("UserName", UserName),
                    DBConnection.getSqlParameter(WarehouseStockModel.COL_InventoryID.Name, InventoryID)
                ).ToList();
        }

        public JsonResult GetList(string InventoryID)
        {
            List<WarehouseStockModel> models = get(db, InventoryID, OperatorController.getUsername(Session));

            string content = @"
                <div class='table-responsive'>
                    <table class='table table-striped table-bordered'>
                        <thead>
                            <tr>
                                <th class='text-center'>Gudang</th>
                                <th class='text-center'>Qty</th>
                            </tr>
                        </thead>
                        <tbody>
            ";

            foreach (var model in models)
            {
                content += @"
                    <tr>
                        <td>" + model.WarehouseName + @"</td>
                        <td class='text-right'>" + string.Format("{0:N0}", model.Stock) + @"</td>
                    </tr>
                ";
            }

            content += "</tbody></table></div>";

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        /******************************************************************************************************************************************************/
    }
}