using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models.Posts
{
    public class Comment
    {
        public Comment(string CommenterName,DateTime CommentedDate,string Text)
        {
            this.CommentedDate = CommentedDate;
            this.CommenterName = CommenterName;
            this.Text = Text;
        }
        public string CommenterName { get; set; }
        public DateTime CommentedDate { get; set; }
        public string Text { get; set; }
    }
}