﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class OwnerWithReportsDto
    {
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }

        public IEnumerable<ReportDto> Reports { get; set; }
    }
}
