using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Configs
{
    public static class MainConfigs
    {
        public const string Url = "http://localhost:55620/";

        //Session keys
        public const string SessionToken = "SessionToken";
        public const string SessionFirstnameToken = "FirstnameToken";
        public const string SessionLastnameToken = "LastnameToken";
        public const string SessionUsernameToken = "UsernameToken";

        //Facebook Auth
        public const string FacebookAppId = "2205509553038336";
        public const string FacebookSecretKey = "c9905f631124307e002d2f07899f48f3";
    }
}