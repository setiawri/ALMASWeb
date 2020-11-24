using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ALMASWeb.Models;
using LIBUtil;

namespace ALMASWeb.Controllers
{
    public class InventoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Inventory
        public ActionResult Index(int? rss, int? InventoryGroup, string InventoryCategory, string InventoryType, int? Warehouse, bool? chkOnlyHasStock, string search)
        {
			ViewBag.RemoveDatatablesStateSave = rss;
			return View(prepareIndex(InventoryGroup, InventoryCategory, InventoryType, Warehouse, chkOnlyHasStock, search));
        }

		// POST: Inventory
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(int? InventoryGroup, string InventoryCategory, string InventoryType, int? Warehouse, bool? chkOnlyHasStock, string search)
		{
			return View(prepareIndex(InventoryGroup, InventoryCategory, InventoryType, Warehouse, chkOnlyHasStock, search));
		}

		private List<InventoryModel> prepareIndex(int? InventoryGroup, string InventoryCategory, string InventoryType, int? Warehouse, bool? chkOnlyHasStock, string search)
		{
			if (!Util.hasAccess(Session, OperatorController.SESSION_OperatorPrivilegeDataManagement_InventoryList))
				RedirectToAction(nameof(HomeController.Index), "Home");

			setViewBag();
			Helper.setFilterViewBag(this, InventoryGroup, InventoryCategory, InventoryType, Warehouse, chkOnlyHasStock, search);

			List<InventoryModel> models = null;
			if (InventoryGroup != null)
				models = get(null, InventoryGroup, InventoryCategory, InventoryType, Warehouse, chkOnlyHasStock, OperatorController.getUsername(Session));

			return models;
		}

		/* METHODS ********************************************************************************************************************************************/

		private List<InventoryModel> get(Guid? id, int? GroupID, string CategoryID, string InventoryType, int? WarehouseID, bool? chkOnlyHasStock, string UserName)
        {
            List<InventoryModel> models = db.Database.SqlQuery<InventoryModel>(sqlGetInventory,
                    DBConnection.getSqlParameter(InventoryModel.COL_InventoryID.Name, id),
                    DBConnection.getSqlParameter(InventoryModel.COL_GroupID.Name, GroupID),
                    DBConnection.getSqlParameter(InventoryModel.COL_CategoryID.Name, CategoryID),
                    DBConnection.getSqlParameter(InventoryModel.COL_TypeID.Name, InventoryType),
                    DBConnection.getSqlParameter(InventoryModel.COL_WarehouseID.Name, WarehouseID),
					DBConnection.getSqlParameter("FILTER_OnlyHasStock", chkOnlyHasStock),
					DBConnection.getSqlParameter(OperatorModel.COL_UserName.Name, UserName)
				).ToList();

			return models;
        }

        public JsonResult getInventoryCategoryList(int? GroupID)
        {
            List<InventoryCategoryModel> models = InventoryCategoryController.get(db, OperatorController.getUsername(Session), GroupID);

            return Json(new { models }, JsonRequestBehavior.AllowGet);
        }

		public JsonResult getCategoryNameAndTypeName(int GroupID)
		{
			InventoryGroupModel model = db.InventoryGroupModel.AsNoTracking().Where(x => x.GroupID == GroupID).FirstOrDefault();

			return Json(new { categoryName = model.CategoryName, typeName = model.TypeName }, JsonRequestBehavior.AllowGet);
		}

		public void setViewBag()
		{
			DataSet dataset = DBConnection.getDataSet("DBContext", sqlPopulateDropdownlists,
					DBConnection.getSqlParameter("UserName", OperatorController.getUsername(Session)));

			ViewBag.Warehouse = Util.getSelectList(dataset.Tables[0], WarehouseModel.COL_WarehouseID.Name, WarehouseModel.COL_WarehouseName.Name);
			ViewBag.WarehouseCount = dataset.Tables[0].Rows.Count;

			ViewBag.InventoryGroup = Util.getSelectList(dataset.Tables[1], InventoryGroupModel.COL_GroupID.Name, InventoryGroupModel.COL_Name.Name);
			ViewBag.InventoryGroupCount = dataset.Tables[1].Rows.Count;
			ViewBag.InventoryCategoryName = dataset.Tables[1].Rows[0][InventoryGroupModel.COL_CategoryName.Name];
			ViewBag.InventoryTypeName = dataset.Tables[1].Rows[0][InventoryGroupModel.COL_TypeName.Name];

			ViewBag.InventoryCategory = Util.getSelectList(dataset.Tables[2], InventoryCategoryModel.COL_CategoryID.Name, InventoryCategoryModel.COL_Name.Name);
			ViewBag.InventoryCategoryCount = dataset.Tables[2].Rows.Count;

			ViewBag.InventoryType = Util.getSelectList(dataset.Tables[3], InventoryTypeModel.COL_TypeID.Name, InventoryTypeModel.COL_Name.Name);
			ViewBag.InventoryTypeCount = dataset.Tables[3].Rows.Count;
		}

		public string sqlPopulateDropdownlists = @"
			DECLARE @WarehouseAllowedList TABLE([WarehouseID] varchar(50),[WarehouseName] varchar(50))

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

			DECLARE @WarehouseList TABLE([WarehouseID] varchar(50),[WarehouseName] varchar(50))

			INSERT INTO @WarehouseList
			SELECT DISTINCT W.[WarehouseID],W.[WarehouseName] 
			FROM @WarehouseAllowedList W,[DWSystem].[WarehouseStock] S,[DWSystem].[Inventory] I
			WHERE W.[WarehouseID] = S.[WarehouseID] 
			AND S.[InventoryID] = I.[InventoryID] 
			AND I.[GroupID] IN(
			SELECT [GroupID] 
			FROM @GroupAllowedList 
			) 

			DECLARE @GroupList TABLE([GroupID] varchar(50),[Group] varchar(50))

			INSERT INTO @GroupList
			SELECT DISTINCT G.[GroupID],G.[Group] 
			FROM @GroupAllowedList G,[DWSystem].[Inventory] I,[DWSystem].[WarehouseStock] S
			WHERE G.[GroupID] = I.[GroupID] 
			AND  I.[InventoryID] = S.[InventoryID]
			AND S.[WarehouseID] IN(
			SELECT DISTINCT [WarehouseID] 
			FROM  @WarehouseAllowedList
			) 

			--# Warehouse #--
			SELECT 
				WarehouseID,
				WarehouseName  
			FROM @WarehouseList
			ORDER BY WarehouseName 


			--# Inventory Group #--
			SELECT DISTINCT 
				InventoryGroup.GroupID,
				InventoryGroup.Code,
				InventoryGroup.Name,
				InventoryGroup.CategoryName,
				InventoryGroup.TypeName 
			FROM @GroupList GroupList
				LEFT JOIN DWSystem.InventoryGroup ON InventoryGroup.GroupID = GroupList.GroupID
			ORDER BY InventoryGroup.Name 


			--# Inventory Category #--
			SELECT DISTINCT 
				InventoryCategory.CategoryID,
				InventoryCategory.Code,
				InventoryCategory.Name 
			FROM DWSystem.InventoryCategory
				LEFT JOIN DWSystem.Inventory ON Inventory.CategoryID = InventoryCategory.CategoryID
				LEFT JOIN DWSystem.WarehouseStock ON WarehouseStock.InventoryID = Inventory.InventoryID
			WHERE 
				Inventory.GroupID IN (
					SELECT GroupID  
					FROM @GroupList
				)
				AND WarehouseStock.WarehouseID IN (
					SELECT WarehouseID  
					FROM @WarehouseList
				)
			ORDER BY InventoryCategory.Name 


			--# InventoryType #--
			SELECT DISTINCT 
				InventoryType.TypeID,
				InventoryType.Code,
				InventoryType.Name 
			FROM DWSystem.InventoryType
				LEFT JOIN DWSystem.Inventory ON Inventory.TypeID = InventoryType.TypeID
				LEFT JOIN DWSystem.WarehouseStock ON WarehouseStock.InventoryID = Inventory.InventoryID
			WHERE 
				Inventory.GroupID IN (
					SELECT GroupID  
					FROM @GroupList
				)
				AND WarehouseStock.WarehouseID IN (
					SELECT WarehouseID  
					FROM @WarehouseList
				)
			ORDER BY InventoryType.Name 
		";

		public string sqlGetInventory = @"
			--DECLARE @UserName as varchar(50) = 'TREATMENT'--'ADMIN'
			--DECLARE @WarehouseID as varchar(50) = ''--2'
			--DECLARE @GroupID as varchar(50) = '1'
			--DECLARE @CategoryID as varchar(50) = ''--P001'
			--DECLARE @TypeID as varchar(50) = ''--P002'

			DECLARE @FilterWarehouseList TABLE([WarehouseID] varchar(50))
			DECLARE @FilterGroupList TABLE([GroupID] varchar(50))
			DECLARE @FilterCategoryList TABLE([CategoryID] varchar(50))
			DECLARE @FilterTypeList TABLE([TypeID] varchar(50))

			INSERT INTO @FilterWarehouseList 
			SELECT [WarehouseID] 
			FROM [DWSystem].[Warehouse] 
			WHERE [WarehouseID] = @WarehouseID 

			INSERT INTO @FilterGroupList 
			SELECT [GroupID] 
			FROM [DWSystem].[InventoryGroup]  
			WHERE [GroupID] = @GroupID 

			INSERT INTO @FilterCategoryList 
			SELECT [CategoryID] 
			FROM [DWSystem].[InventoryCategory]  
			WHERE [CategoryID] = @CategoryID 

			INSERT INTO @FilterTypeList 
			SELECT [TypeID] 
			FROM [DWSystem].[InventoryType]  
			WHERE [TypeID] = @TypeID 

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

			DECLARE @GroupList TABLE([GroupID] varchar(50),[Group] varchar(50))
			INSERT INTO @GroupList
			SELECT DISTINCT G.[GroupID],G.[Group] 
			FROM @GroupAllowedList G,[DWSystem].[Inventory] I,[DWSystem].[WarehouseStock] S
			WHERE G.[GroupID] = I.[GroupID] 
			AND  I.[InventoryID] = S.[InventoryID]
			AND S.[WarehouseID] IN(
			SELECT DISTINCT [WarehouseID] 
			FROM  @WarehouseAllowedList
			) 


			--#Filter Warehouse List#--
			IF (SELECT COUNT(*) FROM @FilterWarehouseList) = 0 
			BEGIN 
				INSERT INTO @FilterWarehouseList
				SELECT [WarehouseID]
				FROM @WarehouseList
			END 


			--#Filter Group List#--
			IF (SELECT COUNT(*) FROM @FilterGroupList) = 0 
			BEGIN 
				INSERT INTO @FilterGroupList
				SELECT DISTINCT [GroupID]
				FROM @GroupList L	
			END 

			--#Filter Category List#--
			IF (SELECT COUNT(*) FROM @FilterCategoryList) = 0 
			BEGIN 
				INSERT INTO @FilterCategoryList 
				SELECT DISTINCT C.[CategoryID]  
				FROM [DWSystem].[InventoryCategory] C,[DWSystem].[Inventory] I,[DWSystem].[WarehouseStock] S
				WHERE C.[CategoryID] = I.[CategoryID] AND I.[InventoryID] = S.[InventoryID] 
				AND I.[GroupID] IN(
				SELECT [GroupID]  
				FROM @GroupList
				)
				AND S.[WarehouseID] IN(
				SELECT [WarehouseID]  
				FROM @WarehouseList
				)
			END 

			--#Filter Type List#--
			IF (SELECT COUNT(*) FROM @FilterTypeList) = 0 
			BEGIN 
				INSERT INTO @FilterTypeList 
				SELECT DISTINCT T.[TypeID]  
				FROM [DWSystem].[InventoryType] T,[DWSystem].[Inventory] I,[DWSystem].[WarehouseStock] S
				WHERE T.[TypeID] = I.[TypeID] AND I.[InventoryID] = S.[InventoryID] 
				AND I.[GroupID] IN(
				SELECT [GroupID]  
				FROM @GroupList
				)
				AND S.[WarehouseID] IN(
				SELECT [WarehouseID]  
				FROM @WarehouseList
				)
			END 


			SELECT
				Inventory.*,
				FilteredInventory.Type AS TypeName,
				FilteredInventory.Stock,
				FilteredInventory.FormattedStock,
				FilteredInventory.Category AS CategoryName,
				FilteredInventory.Packing AS PackingInfo,
				FilteredInventory.PriceTag
			FROM (

					SELECT I.[InventoryID],I.[CategoryID],C.[Name] as [Category],I.[TypeID],T.[Name] as [Type],I.[InventoryName],
					Y.[CurrencySymbol] + ' ' + convert(varchar(50),cast((CASE WHEN ISNULL(I.[PriceUnitTypeID],0) = 0 THEN CASE WHEN I.[ThirdUnitID] IS NULL THEN (CASE WHEN I.[SecUnitID] IS NULL THEN I.[Price] ELSE (I.[SecRatio] * I.[Price]) END) ELSE (I.[ThirdRatio] * I.[Price]) END ELSE I.[Price] END) as money),1) 
					+ ' /' + CASE WHEN ISNULL(I.[PriceUnitTypeID],0) = 0 THEN CASE WHEN I.[ThirdUnitID] IS NULL THEN (CASE WHEN I.[SecUnitID] IS NULL THEN U1.[Code] ELSE U2.[Code] END) ELSE U3.[Code] END ELSE U1.[Code] END as [PriceTag],
					Replace(convert(varchar(50),cast(ISNULL(SUM(S.[Stock]),0) as money),1),'.00','') + ' ' + U1.[Code] as FormattedStock,
					ISNULL(SUM(S.[Stock]),0) as Stock,
					CASE WHEN U3.[Code] IS NOT NULL AND ISNULL(I.[ThirdRatio],0) > 0  THEN U3.[Code] + ' = ' + Replace(convert(varchar(50),cast(ISNULL(I.[ThirdRatio],0) as money),1),'.00','') +  ' ' + U1.[Code] + CHAR(13) + CHAR(10) + 
					U2.[Code] + ' = ' + Replace(convert(varchar(50),cast(ISNULL(I.[SecRatio],0) as money),1),'.00','') +  ' ' + U1.[Code] 
					ELSE 
					CASE WHEN U2.[Code] IS NOT NULL AND ISNULL(I.[SecRatio],0) > 0 THEN U2.[Code] + ' = ' + Replace(convert(varchar(50),cast(ISNULL(I.[SecRatio],0) as money),1),'.00','') +  ' ' + U1.[Code] ELSE '' END 
					END as [Packing] --,I.[Picture] 
					FROM [DWSystem].[Inventory] I LEFT OUTER JOIN [DWSystem].[InventoryUnit] U2 ON I.[SecUnitID] = U2.[UnitID]
					LEFT OUTER JOIN [DWSystem].[InventoryUnit] U3 ON I.[ThirdUnitID] = U3.[UnitID],[DWSystem].[InventoryUnit] U1,
					[DWSystem].[InventoryCategory] C,[DWSystem].[InventoryType] T,[DWSystem].[WarehouseStock] S,
					[DWSystem].[Currency] Y  
					WHERE I.[CategoryID] = C.[CategoryID] AND I.[TypeID] = T.[TypeID] AND I.[InventoryID] = S.[InventoryID] AND I.[Batched] = 0
					AND I.[UnitID] = U1.[UnitID] AND I.[CurrencyID] = Y.[CurrencyID] 
					AND S.[WarehouseID] IN(SELECT [WarehouseID] FROM @FilterWarehouseList)
					AND I.[GroupID] IN(SELECT [GroupID] FROM @FilterGroupList)
					AND I.[CategoryID] IN(SELECT [CategoryID] FROM @FilterCategoryList)
					AND I.[TypeID] IN(SELECT [TypeID] FROM @FilterTypeList)
					GROUP BY I.[InventoryID],I.[CategoryID],C.[Name],I.[TypeID],T.[Name],I.[InventoryName],
					Y.[CurrencySymbol],I.[PriceUnitTypeID],I.[ThirdUnitID],I.[SecUnitID],I.[Price],
					I.[SecRatio],I.[ThirdRatio],U1.[Code],U2.[Code],U3.[Code] --,I.[Picture]
					UNION 
					SELECT I.[InventoryID],I.[CategoryID],C.[Name] as [Category],I.[TypeID],T.[Name] as [Type],I.[InventoryName],
					Y.[CurrencySymbol] + ' ' + convert(varchar(50),cast((CASE WHEN ISNULL(I.[PriceUnitTypeID],0) = 0 THEN CASE WHEN I.[ThirdUnitID] IS NULL THEN (CASE WHEN I.[SecUnitID] IS NULL THEN I.[Price] ELSE (I.[SecRatio] * I.[Price]) END) ELSE (I.[ThirdRatio] * I.[Price]) END ELSE I.[Price] END) as money),1) 
					+ ' /' + CASE WHEN ISNULL(I.[PriceUnitTypeID],0) = 0 THEN CASE WHEN I.[ThirdUnitID] IS NULL THEN (CASE WHEN I.[SecUnitID] IS NULL THEN U1.[Code] ELSE U2.[Code] END) ELSE U3.[Code] END ELSE U1.[Code] END as [PriceTag],
					Replace(convert(varchar(50),cast(ISNULL(SUM(S.[Stock]),0) as money),1),'.00','') + ' ' + U1.[Code] as FormattedStock,
					ISNULL(SUM(S.[Stock]),0) as Stock,
					CASE WHEN U3.[Code] IS NOT NULL AND ISNULL(I.[ThirdRatio],0) > 0  THEN U3.[Code] + ' = ' + Replace(convert(varchar(50),cast(ISNULL(I.[ThirdRatio],0) as money),1),'.00','') +  ' ' + U1.[Code] + CHAR(13) + CHAR(10) + 
					U2.[Code] + ' = ' + Replace(convert(varchar(50),cast(ISNULL(I.[SecRatio],0) as money),1),'.00','') +  ' ' + U1.[Code] 
					ELSE 
					CASE WHEN U2.[Code] IS NOT NULL AND ISNULL(I.[SecRatio],0) > 0 THEN U2.[Code] + ' = ' + Replace(convert(varchar(50),cast(ISNULL(I.[SecRatio],0) as money),1),'.00','') +  ' ' + U1.[Code] ELSE '' END 
					END as [Packing] --,I.[Picture] 
					FROM [DWSystem].[Inventory] I LEFT OUTER JOIN [DWSystem].[InventoryUnit] U2 ON I.[SecUnitID] = U2.[UnitID]
					LEFT OUTER JOIN [DWSystem].[InventoryUnit] U3 ON I.[ThirdUnitID] = U3.[UnitID],[DWSystem].[InventoryUnit] U1,
					[DWSystem].[InventoryCategory] C,[DWSystem].[InventoryType] T,[DWSystem].[WarehouseStockBatch] S,
					[DWSystem].[Currency] Y  
					WHERE I.[CategoryID] = C.[CategoryID] AND I.[TypeID] = T.[TypeID] AND I.[InventoryID] = S.[InventoryID] AND I.[Batched] = 1
					AND I.[UnitID] = U1.[UnitID] AND I.[CurrencyID] = Y.[CurrencyID] 
					AND S.[WarehouseID] IN(SELECT [WarehouseID] FROM @FilterWarehouseList)
					AND I.[GroupID] IN(SELECT [GroupID] FROM @FilterGroupList)
					AND I.[CategoryID] IN(SELECT [CategoryID] FROM @FilterCategoryList)
					AND I.[TypeID] IN(SELECT [TypeID] FROM @FilterTypeList)
					GROUP BY I.[InventoryID],I.[CategoryID],C.[Name],I.[TypeID],T.[Name],I.[InventoryName],
					Y.[CurrencySymbol],I.[PriceUnitTypeID],I.[ThirdUnitID],I.[SecUnitID],I.[Price],
					I.[SecRatio],I.[ThirdRatio],U1.[Code],U2.[Code],U3.[Code] --,I.[Picture]			
				
				) FilteredInventory
				LEFT JOIN DWSystem.Inventory ON Inventory.InventoryID = FilteredInventory.InventoryID
			WHERE 1=1
				AND (@FILTER_OnlyHasStock IS NULL OR @FILTER_OnlyHasStock = 0 OR FilteredInventory.Stock > 0)
			ORDER BY [InventoryName]  
        ";

		/******************************************************************************************************************************************************/
	}
}