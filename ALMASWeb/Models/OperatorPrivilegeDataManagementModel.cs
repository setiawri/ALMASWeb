using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.OperatorPrivilegeDataManagement")]
    public class OperatorPrivilegeDataManagementModel
    {
        [Key]
        public string UserName { get; set; }

        public bool InventoryList { get; set; }
    }
}