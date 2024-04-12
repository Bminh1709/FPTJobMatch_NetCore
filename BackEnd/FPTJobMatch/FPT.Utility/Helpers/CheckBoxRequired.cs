using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Utility.Helpers
{
    public class CheckBoxRequired : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is bool isChecked)
            {
                return isChecked;
            }
            return false;
        }
    }
}
