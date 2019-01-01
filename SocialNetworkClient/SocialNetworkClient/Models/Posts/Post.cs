using SocialNetworkClient.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class Post
    {
        public string Text { get; set; }
        public string PosterName { get; set; }
        public int Likes { get; set; }
        public List<Comment> Comments { get; set; }
        public string ImageUrl { get; set; }
        public Post(string PosterName,string Text, int Likes, string ImageUrl, List<Comment> Comments = null)
        {
            this.PosterName = PosterName;
            this.Text = Text;
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