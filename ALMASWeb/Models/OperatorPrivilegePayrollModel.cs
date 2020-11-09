using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.OperatorPrivilegePayroll")]
    public class OperatorPrivilegePayrollModel
    {
        [Key]
        [Required]
        public string UserName { get; set; }

        public bool PayrollModule { get; set; }

        public bool Approval { get; set; }
    }
}