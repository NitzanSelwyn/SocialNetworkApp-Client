using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Validators
{
    class BirthDateValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime birthDate;

            if (value == null)
                return ValidationResult.Success;
            try
            {
                birthDate = Convert.ToDateTime(value);
            }
            catch (FormatException)
            {
                return new ValidationResult("ConversionError");
            }

            if (ValidDate(birthDate))
                return ValidationResult.Success;
            else
                return new ValidationResult("Please Enter a valid date of birth");
        }

        public bool ValidDate(DateTime date)//V
        {
            if (date >= DateTime.Now.AddDays(-1) || date < Convert.ToDateTime("01/01/1900") || date == Convert.ToDateTime("01/01/01"))
            {
                return false;
            }
            return true;
        }
    }
}