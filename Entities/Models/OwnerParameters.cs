using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class OwnerParameters : QueryStringParameters
    {
        public OwnerParameters()
        {
            OrderBy = "name";
        }

        public uint MinYearOfBirth { get; set; } = 0;
        public uint MaxYearOfBirth { get; set; } = (uint)DateTime.Now.Year;

        public bool ValidYearRange => MaxYearOfBirth > MinYearOfBirth;

        public string Name { get; set; }
    }
}
