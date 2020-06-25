using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class ReportDto
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "ReportType is required")]
        [StringLength(45, ErrorMessage = "ReportType cannot be longer than 45 characters")]
        public string ReportType { get; set; }
    }
}
