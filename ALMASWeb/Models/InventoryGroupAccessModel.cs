using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.InventoryGroupAccess")]
    public class InventoryGroupAccessModel
    {
        [Key]
        public string UserName { get; set; }
        public static ModelMember COL_UserName = new ModelMember { Name = "UserName", Display = "" };

        public int GroupID { get; set; }

    }
}