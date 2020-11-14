﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    public class InventoryModel
    {
        /* DATABASE COLUMNS ***********************************************************************************************************************************/

        public string InventoryID { get; set; }
        public static ModelMember COL_InventoryID = new ModelMember { Name = "InventoryID", Display = "" };

        public bool AutoGenerated { get; set; }

        public int? GroupID { get; set; }
        public static ModelMember COL_GroupID = new ModelMember { Name = "GroupID", Display = "" };

        public string CategoryID { get; set; }
        public static ModelMember COL_CategoryID = new ModelMember { Name = "CategoryID", Display = "" };

        public string TypeID { get; set; }
        public static ModelMember COL_TypeID = new ModelMember { Name = "TypeID", Display = "" };

        public string InventoryName { get; set; }

        public string Description { get; set; }

        public bool Batched { get; set; }

        public int? BatchUnitID { get; set; }

        public bool isService { get; set; }

        public int UnitID { get; set; }

        public int? SecUnitID { get; set; }

        public int? SecRatio { get; set; }

        public int? ThirdUnitID { get; set; }

        public int? ThirdRatio { get; set; }

        public long MinimumStock { get; set; }

        public int CurrencyID { get; set; }

        public int? PriceUnitTypeID { get; set; }

        public double Cost { get; set; }

        public double Price { get; set; }

        public bool IncTax { get; set; }

        public int ProcessLevel { get; set; }

        public double AdditionalCost { get; set; }

        public bool FlagOn { get; set; }

        public bool Deleted { get; set; }

        public DateTime RecordDate { get; set; }

        public DateTime? LastUpdated { get; set; }

        public byte[] Picture { get; set; }

        public string Filename { get; set; }

        public string NoItem { get; set; }

        public string Specification { get; set; }

        public string NoUPC { get; set; }

        public string NoSKU { get; set; }

        public string NoUCC { get; set; }

        public string NoUCCIP { get; set; }

        public string NoNW { get; set; }

        public string NoGW { get; set; }

        public string NoCBM { get; set; }

        public double? Weight { get; set; }

        public double? WeightNet { get; set; }

        public double? Length { get; set; }

        public double? Width { get; set; }

        public double? Height { get; set; }

        public string PriceLabel { get; set; }

        public double? CoilLength { get; set; }

        public string HSCode { get; set; }


        /* ADDITIONAL PROPERTIES ******************************************************************************************************************************/


        public string PriceTag { get; set; }

        public string CategoryName { get; set; }

        public string TypeName { get; set; }

        public bool HasComponent { get; set; }

        public bool? HasSpecification { get; set; }

        public string Stock { get; set; }

        public string FormattedStock { get; set; }

        public string PackingInfo { get; set; }

        public int? WarehouseID { get; set; }
        public static ModelMember COL_WarehouseID = new ModelMember { Name = "WarehouseID", Display = "" };

        /******************************************************************************************************************************************************/

    }
}