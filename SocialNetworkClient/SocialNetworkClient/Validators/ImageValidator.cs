using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Validators
{
    public class ImageValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HttpPostedFileBase file;

            if (value == null)
                return ValidationResult.Success;
            try
            {
                file = (HttpPostedFileBase)value;
            }
            catch (FormatException)
            {
                return new ValidationResult("ConversionError");
            }

            if (IsImage(file))
                return ValidationResult.Success;
            else
                return new ValidationResult("Please Enter a valid date of birth");
        }

        public bool IsImage(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    if ((Path.GetExtension(file.FileName).ToLower() == ".jpg") ||
                            (Path.GetExtension(file.FileName).ToLower() == ".png") ||
                            (Path.GetExtension(file.FileName).ToLower() == ".gif") ||
                            (Path.GetExtension(file.FileName).ToLower() == ".jpeg"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }//V
    }
}