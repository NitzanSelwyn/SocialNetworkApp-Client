using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Configs
{
    public static class ApiConfigs
    {
        public const string userLoginRoute = "Api/Users/userlogin";
        public const string userRegisterRoute = "Api/Users/register";

        public const string PostNewMessage = "Api/Post";
        public const string GetUsersPosts = "Api/Post/UsersPosts";
        public const string GetFolowersPosts = "Api/Post/FolowersPosts";
        public const string EditPost = "Api/Post/edit";
        public const string DeletePost = "Api/Post";
    }
}