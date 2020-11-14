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

        public static List<WarehouseStockModel> get(DBContext db, string InventoryID, string WarehouseID, string UserName)
        {
            return db.Database.SqlQuery<WarehouseStockModel>(sqlWarehouseStock,
                    DBConnection.getSqlParameter("UserName", UserName),
                    DBConnection.getSqlParameter(WarehouseStockModel.COL_InventoryID.Name, InventoryID),
                    DBConnection.getSqlParameter(WarehouseStockModel.COL_WarehouseID.Name, WarehouseID)
                ).ToList();
        }

        public JsonResult GetList(string InventoryID, string WarehouseID)
        {
            List<WarehouseStockModel> models = get(db, InventoryID, WarehouseID, OperatorController.getUsername(Session));

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

        public static string sqlWarehouseStock = @"
            --DECLARE @UserName as varchar(50)
            --SET @UserName = 'ADMIN'--BUYER'

            --DECLARE @WarehouseID as varchar(50) 
            --SET @WarehouseID = ''--2'

            --DECLARE @InventoryID as varchar(50) 
            --SET @InventoryID = '15537-MP WP'

            DECLARE @FilterWarehouseList TABLE([WarehouseID] int)

            INSERT INTO @FilterWarehouseList 
            SELECT [WarehouseID] 
            FROM [DWSystem].[Warehouse] 
            WHERE [WarehouseID] = @WarehouseID 



            DECLARE @WarehouseAllowedList TABLE([WarehouseID] varchar(50),[Warehouse] varchar(50))

            INSERT INTO @WarehouseAllowedList
            SELECT W.[WarehouseID],W.[WarehouseName]  
            FROM [DWSystem].[Warehouse] W,[DWSystem].[WarehouseAccess] A
            WHERE W.[WarehouseID] = A.[WarehouseID] AND A.[UserName] = @UserName 

            IF (SELECT COUNT(*) FROM @WarehouseAllowedList) = 0 
            BEGIN 
	            INSERT INTO @WarehouseAllowedList
	            SELECT [WarehouseID],[WarehouseName]  
	            FROM [DWSystem].[Warehouse]
            END 

            --SELECT * 
            --FROM @WarehouseAllowedList 

            DECLARE @GroupAllowedList TABLE([GroupID] varchar(50),[Group] varchar(50))

            INSERT INTO @GroupAllowedList
            SELECT A.[GroupID],G.[Name] 
            FROM [DWSystem].[InventoryGroup] G,[DWSystem].[InventoryGroupAccess] A
            WHERE G.[GroupID] = A.[GroupID] AND A.[UserName] = @UserName 

            IF (SELECT COUNT(*) FROM @GroupAllowedList) = 0 
            BEGIN 
	            INSERT INTO @GroupAllowedList
	            SELECT [GroupID],[Name] 
	            FROM [DWSystem].[InventoryGroup] 
            END 

            --SELECT * 
            --FROM @GroupAllowedList  

            DECLARE @WarehouseList TABLE([WarehouseID] varchar(50),[Warehouse] varchar(50))
            INSERT INTO @WarehouseList
            SELECT DISTINCT W.[WarehouseID],W.[Warehouse] 
            FROM @WarehouseAllowedList W,[DWSystem].[WarehouseStock] S,[DWSystem].[Inventory] I
            WHERE W.[WarehouseID] = S.[WarehouseID] 
            AND S.[InventoryID] = I.[InventoryID] 
            AND I.[GroupID] IN(
            SELECT [GroupID] 
            FROM @GroupAllowedList 
            ) 

            --#Filter Warehouse List#--
            IF (SELECT COUNT(*) FROM @FilterWarehouseList) = 0 
            BEGIN 
	            INSERT INTO @FilterWarehouseList
	            SELECT [WarehouseID]
	            FROM @WarehouseList
            END 

            SELECT L.[WarehouseID],W.[WarehouseName],ISNULL(SUM(S.[Stock]),0) as Stock
            FROM [DWSystem].[Warehouse] W,[DWSystem].[WarehouseStock] S,[DWSystem].[Inventory] I,@FilterWarehouseList L
            WHERE W.[WarehouseID] = S.[WarehouseID] 
            AND I.[InventoryID] = S.[InventoryID] 
            AND W.[WarehouseID] = L.[WarehouseID] 
            AND S.[InventoryID] = @InventoryID 
            AND I.[Batched] = 0 
            GROUP BY L.[WarehouseID],W.[WarehouseName]
            UNION 
            SELECT L.[WarehouseID],W.[WarehouseName],ISNULL(SUM(S.[Stock]),0) as Stock
            FROM [DWSystem].[Warehouse] W,[DWSystem].[WarehouseStockBatch] S,[DWSystem].[Inventory] I,@FilterWarehouseList L
            WHERE W.[WarehouseID] = S.[WarehouseID] 
            AND I.[InventoryID] = S.[InventoryID] 
            AND W.[WarehouseID] = L.[WarehouseID] 
            AND S.[InventoryID] = @InventoryID 
            AND I.[Batched] = 1 
            GROUP BY L.[WarehouseID],W.[WarehouseName]
            ORDER BY [WarehouseName]

        ";
        /******************************************************************************************************************************************************/
    }
}