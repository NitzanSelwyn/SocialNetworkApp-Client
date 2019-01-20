using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models.Posts;
using SocialNetworkClient.Models.RequestsAndResponses;
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
            PostList = new List<Post>();
            UsersRep = new List<UserRepresentation>();
            Notifications = new List<Notification>();
        }
        public User LoggedInUser { get; set; }
        public UserRegister UserRegister { get; set; }
        public UserLogin UserLogin { get; set; }
        public PostUpload Post { get; set; }
        public EditPassword EditPassword { get; set; }
        public List<Post> PostList { get; set; }
        public string SearchInput { get; set; }
        public List<User> SearchedUsers { get; set; }
        public UserViewModel UserToView { get; set; }
        public List<UserRepresentation> UsersRep { get; set; }
        public List<Notification> Notifications { get; set; }
        public int PostCounter { get; set; }
    }
}