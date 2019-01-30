using SocialNetworkClient.Configs;
using SocialNetworkClient.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SocialNetworkClient.Services
{
    public class InputsValidator : IInputsValidator
    {
        public string ValidateStrInput(string fieldName, string input, int minLength, int maxLength)
        {
            //validates an input, returns an empty string if the input has been approved
            string returnStr = "";
            if (string.IsNullOrEmpty(input))
            {
                returnStr = $"Please insert a {fieldName}";
            }
            else if (input.Length < minLength || input.Length > maxLength)
            {
                returnStr = $"{fieldName} length must be between {minLength} and {maxLength} chars";
            }
            return returnStr;
        }

        public string ValidateIntInput(string fieldName, string input, int minValue, int maxValue)
        {
            //validates an input, returns an empty string if the input has been approved
            string returnStr = "";
            int outNum;
            if (!int.TryParse(input, out outNum))
            {
                returnStr = $"{fieldName} must be between {minValue} and {maxValue} digits only";
            }

            else if (outNum < minValue || outNum > maxValue)
            {
                returnStr = $"{fieldName} must be between {minValue} and {maxValue} digits only";
            }
            return returnStr;
        }

        public string ValidateYearOfBirthInput(string fieldName, string input)
        {
            string returnStr = "";
            int inputAsInt;
            bool parseSuccess = int.TryParse(input, out inputAsInt);
            if (input.Length != 4)
            {
                returnStr = $"{fieldName} must be between {0} and {DateTime.Now.Year}. 4 digits only";
            }
            if (parseSuccess)
            {
                if (inputAsInt < 0 || inputAsInt > DateTime.Now.Year)
                {
                    returnStr = $"{fieldName} must be between {0} and {DateTime.Now.Year}. 4 digits only";
                }
            }
            return returnStr;
        }

        public string ValidateIntInput(string fieldName, int input, int minValue, int maxValue)
        {
            //validates an input, returns an empty string if the input has been approved
            string returnStr = "";

            if (input < minValue || input > maxValue)
            {
                returnStr = $"{fieldName} must be between {minValue} and {maxValue} digits only";
            }
            return returnStr;
        }

        public string ValidateDoubleInput(string fieldName, string input, int minValue, int maxValue)
        {
            //validates an input, returns an empty string if the input has been approved
            string returnStr = "";
            double outNum;
            if (!double.TryParse(input, out outNum))
            {
                returnStr = $"{fieldName} must be between {minValue} and {maxValue} digits only";
            }
            else if (outNum == 0)
            {
                returnStr = $"Please insert a {fieldName}";
            }
            else if (outNum < minValue || outNum > maxValue)
            {
                returnStr = $"{fieldName} must be between {minValue} and {maxValue} digits only";
            }
            return returnStr;
        }

        public string ValidateDoubleInput(string fieldName, double input, int minValue, int maxValue)
        {
            //validates an input, returns an empty string if the input has been approved
            string returnStr = "";

            if (input < minValue || input > maxValue)
            {
                returnStr = $"{fieldName} must be between {minValue} and {maxValue} digits only";
            }
            return returnStr;
        }

        public string ValidateDateInput(string fieldName, DateTime input)
        {
            //validates an input, returns an empty string if the input has been approved
            string returnStr = "";
            if (input >= DateTime.Now)
            {
                returnStr = $"Please insert a valid {fieldName}";
            }

            return returnStr;
        }

        public string ValidateRegEx(string fieldName,string ex,string input)
        {
            //validates an input to a regular expresssion
            var match = Regex.Match(input, ex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return "";
            }
            else
            {
                return $"Please insert a valid {fieldName}";
            }
        }
    }
}