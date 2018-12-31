using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class UserRegister
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public DateTime BirthDate { get; set; }
        public string  Address { get; set; }
        public string Email { get; set; }
        public string WorkLocation { get; set; }
    }
}