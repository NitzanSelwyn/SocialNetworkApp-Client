using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Contracts
{
    public interface IInputsValidator
    {
        string ValidateStrInput(string fieldName, string input, int minLength, int maxLength);
        string ValidateRegEx(string fieldName, string ex, string input);
        string ValidateDateInput(string fieldName, DateTime input);
    }
}