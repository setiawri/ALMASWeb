using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.InventoryType")]
    public class InventoryTypeModel
    {
        [Key]
        public string TypeID { get; set; }
        public static ModelMember COL_TypeID = new ModelMember { Name = "TypeID", Display = "" };

        public int No { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "" };

        public int GroupID { get; set; }
        public static ModelMember COL_GroupID = new ModelMember { Name = "GroupID", Display = "" };
    }
}