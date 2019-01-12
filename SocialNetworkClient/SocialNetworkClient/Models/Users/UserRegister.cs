using SocialNetworkClient.Configs;
using SocialNetworkClient.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class UserRegister
    {
        public UserRegister()
        {

        }
        [Required(ErrorMessage = "* Please Enter a valid user name")]
        [StringLength(InputsConfigs.MaxGenLen)]
        [DisplayName("User Name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "* Please Enter a first name")]
        [StringLength(InputsConfigs.MaxGenLen)]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "* Please Enter a last name")]
        [StringLength(InputsConfigs.MaxGenLen)]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "* Please Enter a Password")]
        [StringLength(InputsConfigs.MaxGenLen)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "* Please Enter a Confirmation password")]
        [StringLength(InputsConfigs.MaxGenLen)]
        [DisplayName("Confirmation Password")]
        public string PasswordConfirm { get; set; }

        [BirthDateValidator]
        [Required(ErrorMessage = "* Please enter a valid date of birth")]
        [DataType(DataType.Date)]
        [DisplayName("Date of birth")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "* Please Enter an Address")]
        [StringLength(InputsConfigs.MaxAddressLen)]
        [DisplayName("Address")]
        public string  Address { get; set; }

        [Required(ErrorMessage = "* Please Enter a valid email adress")]
        [StringLength(InputsConfigs.MaxEmailLen)]
        [DisplayName("Email Adress")]
        [RegularExpression(InputsConfigs.EmailRegEx, ErrorMessage = "* Please Enter a valid email adress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* Please Enter a Work location")]
        [StringLength(InputsConfigs.MaxAddressLen)]
        [DisplayName("Work Location")]
        public string WorkLocation { get; set; }
        
    }
}