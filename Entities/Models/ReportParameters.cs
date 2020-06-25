using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class ReportParameters : QueryStringParameters
    {
        public ReportParameters()
        {
            OrderBy = "DateCreated";
        }
    }
}
