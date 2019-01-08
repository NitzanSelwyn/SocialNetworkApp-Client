using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Configs
{
    public static class ApiConfigs
    {
        public const string UserLoginRoute = "Api/Users/UserLogin";
        public const string UserRegisterRoute = "Api/Users/UserRegister";
        public const string UsernameExistsRoute = "Api/Users/UsernameExists";

        public const string GetTokenRoute = "Api/Users/GetToken";
        public const string ValidateTokenRoute = "Api/Users/ValidateToken";

        public const string GetMyUserRoute = "Api/users/GetUserByToken";
    }
}