using SocialNetworkClient.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class Post
    {
        public string PosterName { get; set; }
        public int Likes { get; set; }
        public List<Comment> Comments { get; set; }
        public int? ImageUrl { get; set; }
        public Post(string PosterName, int Likes, int? ImageUrl, List<Comment> Comments = null)
        {
            this.PosterName = PosterName;
            this.Likes = Likes;
            this.ImageUrl = ImageUrl;
            if (Comments != null)
            {
                this.Comments = Comments;
            }
            else
            {
                this.Comments = new List<Comment>();
            }
           
        }
    }
}