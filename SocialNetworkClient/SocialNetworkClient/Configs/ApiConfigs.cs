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
        public const string GetFollowingPosts = "Api/Post/FollowingPosts";
        public const string DeletePost = "Api/Post";
        //USERS
        public const string UserLoginRoute = "Api/Users/UserLogin";
        public const string UserRegisterRoute = "Api/Users/UserRegister";
        public const string UsernameExistsRoute = "Api/Users/UsernameExists";
        public const string EditUserDetailsRoute = "Api/Users/EditUserDetails";

        //AUTH AND TOKENS
        public const string GetTokenRoute = "Api/Users/GetToken";
        public const string ValidateTokenRoute = "Api/Users/ValidateToken";
        public const string GetMyUserRoute = "Api/users/GetUserByToken";

        //SETTINGS
        public const string ManageRequestRoute = "Api/Settings/ManageRequest";
        public const string GetBlockedUsers = "Api/Settings/GetBlockedUsers";
    }
}