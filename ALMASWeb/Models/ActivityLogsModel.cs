using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALMASWeb.Models
{
    [Table("DWSystem.ActivityLogs")]
    public class ActivityLogsModel
    {
        public Guid Id { get; set; }

        public Guid ReffId { get; set; }

        public DateTime Timestamp { get; set; }

        [Required]
        public string Description { get; set; }

        public int? Operator_ID { get; set; }
    }
}