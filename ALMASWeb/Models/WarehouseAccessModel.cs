using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.WarehouseAccess")]
    public class WarehouseAccessModel
    {
        [Key]
        public string UserName { get; set; }
        public static ModelMember COL_UserName = new ModelMember { Name = "UserName", Display = "" };

        public int WarehouseID { get; set; }

    }
}