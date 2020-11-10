using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.Warehouse")]
    public class WarehouseModel
    {
        [Key]
        public int WarehouseID { get; set; }
        public static ModelMember COL_WarehouseID = new ModelMember { Name = "WarehouseID", Display = "" };

        [Required]
        public string WarehouseName { get; set; }
        public static ModelMember COL_WarehouseName = new ModelMember { Name = "WarehouseName", Display = "" };

        [Required]
        public string Detail { get; set; }

        [Required]
        public bool isProduct { get; set; }

        [Required]
        public bool isMaterial { get; set; }

        [Required]
        public bool isWIP { get; set; }

        [Required]
        public bool isWaste { get; set; }

        [Required]
        public bool isSparePart { get; set; }

        [Required]
        public bool isOfficeInventory { get; set; }

        [Required]
        public bool? BSWarehouse { get; set; }
    }
}