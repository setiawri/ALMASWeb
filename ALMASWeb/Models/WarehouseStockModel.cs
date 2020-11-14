using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.WarehouseStock")]
    public class WarehouseStockModel
    {
        /* DATABASE COLUMNS ***********************************************************************************************************************************/

        [Key]
        public int WarehouseID { get; set; }
        public static ModelMember COL_WarehouseID = new ModelMember { Name = "WarehouseID", Display = "" };

        [Required]
        public string InventoryID { get; set; }
        public static ModelMember COL_InventoryID = new ModelMember { Name = "InventoryID", Display = "" };

        public long Stock { get; set; }

        /* ADDITIONAL PROPERTIES ******************************************************************************************************************************/

        public string WarehouseName { get; set; }

        /******************************************************************************************************************************************************/
    }
}