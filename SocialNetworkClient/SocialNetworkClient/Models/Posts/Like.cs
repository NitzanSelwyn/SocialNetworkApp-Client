﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models.Posts
{
    public class Like
    {
        public string UserName { get; set; }

        public string postId { get; set; }

        public List<string> UsersWhoLiked { get; set; }

        public Like()
        {
            UsersWhoLiked = new List<string>();
        }
    }
}