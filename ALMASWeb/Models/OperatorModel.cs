using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.Operator")]
    public class OperatorModel
    {
        public int ID { get; set; }

        [Required]
        public string UserName { get; set; }
        public static ModelMember COL_UserName = new ModelMember { Name = "UserName", Display = "" };

        [Required]
        public string Password { get; set; }

        public bool Supervisor { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public bool Active { get; set; }

        public int? DivisionID { get; set; }

        public int? ThemeID { get; set; }
    }
}