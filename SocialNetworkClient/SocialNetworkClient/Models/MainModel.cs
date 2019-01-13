using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models.Posts;
using SocialNetworkClient.Models.Users;
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
            BlockedUsers = new Dictionary<string, string>();
            PostList = new List<Post>();
        }
        public User LoggedInUser { get; set; }
        public UserRegister UserRegister { get; set; }
        public UserLogin UserLogin { get; set; }
        public PostUpload Post { get; set; }
        public EditPassword EditPassword { get; set; }
        public Dictionary<string,string> BlockedUsers { get; set; }
        public List<Post> PostList { get; set; }
    }
}