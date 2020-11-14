using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.InventoryGroup")]
    public class InventoryGroupModel
    {
        [Key]
        public int GroupID { get; set; }
        public static ModelMember COL_GroupID = new ModelMember { Name = "GroupID", Display = "" };

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "" };

        [Required]
        public string CategoryName { get; set; }
        public static ModelMember COL_CategoryName = new ModelMember { Name = "CategoryName", Display = "" };

        [Required]
        public string TypeName { get; set; }
        public static ModelMember COL_TypeName = new ModelMember { Name = "TypeName", Display = "" };

        [Required]
        public bool HasComponent { get; set; }

        public bool? HasSpecification { get; set; }

        public int? TypeID { get; set; }
    }
}