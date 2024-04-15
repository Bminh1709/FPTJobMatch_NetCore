using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Utility.Helpers
{
    public class GreaterThanToday : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateOnly deadlineDate)
            {
                return deadlineDate > DateOnly.FromDateTime(DateTime.Today);
            }

            return true;
        }
    }
}
