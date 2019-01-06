using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class MainModel : IMainModel
    {
        public MainModel()
        {

        }
        public User LoggedInUser { get; set; }
        public UserRegister UserRegister { get; set; }
        public UserLogin UserLogin { get; set; }
        public PostUpload Post { get; set; }
    }
}