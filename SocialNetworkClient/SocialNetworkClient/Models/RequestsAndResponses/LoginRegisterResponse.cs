using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models.Users
{
    public class LoginRegisterResponse
    {
        public LoginRegisterResponse()
        {

        }
        public string token { get; set; }
        public User user { get; set; }
        public LoginRegisterResponse(string token, User user)
        {
            this.token = token;
            this.user = user;
        }
    }
}