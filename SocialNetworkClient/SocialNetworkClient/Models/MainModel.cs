using SocialNetworkClient.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class MainModel:IMainModel
    {
        public User LoggedInUser { get; set; }
        public UserRegister UserRegister { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}