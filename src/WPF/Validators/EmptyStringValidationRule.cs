using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NBsoft.Appointment.WPF.Validators
{
    class EmptyStringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Object cannot be null");

            bool canConvert = !string.IsNullOrEmpty(value.ToString());
            return new ValidationResult(canConvert, "String cannot be empty");
        }
    }
}
