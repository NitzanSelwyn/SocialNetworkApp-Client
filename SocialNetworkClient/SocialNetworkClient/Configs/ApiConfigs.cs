﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Configs
{
    public static class ApiConfigs
    {
        public const string userLoginRoute = "Api/Users/userlogin";
        public const string userRegisterRoute = "Api/Users/register";


        //POSTS
        public const string PostNewMessage = "Api/Post";
        public const string GetUsersPosts = "Api/Post/UsersPosts";
        public const string GetFollowingPosts = "Api/Post/FollowingPosts";
        public const string DeletePost = "Api/Post";
        public const string Like = "Api/Like";
        public const string UnLike = "Api/UnLike";
        public const string GetUsersWhoLiked = "Api/UsersWhoLiked";
        public const string GetPostsComments = "Api/GetPostsComments";
        public const string CommentOnPost = "Api/Comment";
        public const string GetTheUsersThatIFollow = "Api/GetTheUsersThatIFollow";
        public const string GetTheUserThatFollowMe = "Api/GetTheUserThatFollowMe";


        //USERS
        public const string UserLoginRoute = "Api/Users/UserLogin";
        public const string UserRegisterRoute = "Api/Users/UserRegister";
        public const string UsernameExistsRoute = "Api/Users/UsernameExists";
        public const string EditUserDetailsRoute = "Api/Users/EditUserDetails";
        public const string SearchUsersRoute = "Api/Users/SearchForUsers";
        public const string BlockedByUsersRoute = "Api/Users/BlockedByUser";
        public const string GetUserByUsername = "Api/Users/GetUserByUsername";
        public const string FacebookLoginRoute = "Api/Users/FacebookLogin";

        public const string GetNotificationCount = "Api/Users/GetNotificationCount";
        public const string GetNotifications = "Api/Users/GetNotifications";
        public const string ClearNotificationsRoute = "Api/User/ClearNotifications";

        //AUTH AND TOKENS
        public const string GetTokenRoute = "Api/Users/GetToken";
        public const string ValidateTokenRoute = "Api/Users/ValidateToken";
        public const string GetMyUserRoute = "Api/users/GetUserByToken";

        //SETTINGS
        public const string ManageRequestRoute = "Api/Settings/ManageRequest";
        public const string GetBlockedUsers = "Api/Settings/GetBlockedUsers";
        public const string EditUserPasswordRoute = "Api/Settings/ChangePassword";
        public const string GetFollowingUsers = "Api/Settings/GetFollowingUsers";
        public const string GetUsersThatFollowsMe = "Api/Settings/GetUsersThatFollowsMe";

       
    }
}