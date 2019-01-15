using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models.Users
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public bool FollowingThisUser { get; set; }
        public List<Post> Posts { get; set; }
        public UserViewModel()
        {
            Posts = new List<Post>();
        }
        public UserViewModel(string Username, string FullName, bool? FollowingThisUser, List<Post> Posts = null)
        {
            this.Username = Username;
            this.FullName = FullName;
            if (FollowingThisUser != null)
            {
                this.FollowingThisUser = (bool)FollowingThisUser;
            }
            if (Posts == null)
            {
                Posts = new List<Post>();
            }
            else
            {
                this.Posts = Posts;
            }
        }
    }
}