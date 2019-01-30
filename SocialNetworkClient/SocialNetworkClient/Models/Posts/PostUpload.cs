using SocialNetworkClient.Configs;
using SocialNetworkClient.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models.Posts
{
    public class PostUpload
    {
        [DisplayName("Post's Text")]
        [Required(ErrorMessage = "* Please fill the text of the post")]
        [DataType(DataType.MultilineText)]
        [StringLength(InputsConfigs.MaxPostTextLen)]
        public string Text { get; set; }

        [DisplayName("Optional Image")]
        [ImageValidator]
        public HttpPostedFileBase Image { get; set; }

        public string Mantioned { get; set; }
    }
}