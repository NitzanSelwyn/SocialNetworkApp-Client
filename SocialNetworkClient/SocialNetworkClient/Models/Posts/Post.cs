﻿using SocialNetworkClient.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models
{
    public class Post
    {
        public string Author { get; set; }

        public string Content { get; set; }

        public byte[] Image { get; set; }

        public string ImageLink { get; set; }

        public Post()
        {

        }

        public List<Comment> CommentList { get; set; }

    }
}