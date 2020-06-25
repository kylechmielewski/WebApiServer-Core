using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Report")]
    public class Report
    {
        [Column("ReportId")]
        public Guid ReportId { get; set; }

        [Column("DateCreated")]
        [Required(ErrorMessage = "DateTime created is required")]
        public DateTime DateCreated { get; set; }

        [Column("ReportType")]
        [Required(ErrorMessage = "Report Type is required")]
        public string ReportType { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }
        public Owner Owner { get; set; }
    }
}
