using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASKITEWeb.Models
{
    [Table("DWSystem.OperatorPrivilegeDataManagement")]
    public class OperatorPrivilegeDataManagementModel
    {
        [Key]
        public string UserName { get; set; }

        public bool InventoryList { get; set; }
    }
}