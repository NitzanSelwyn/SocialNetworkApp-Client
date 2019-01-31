using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Configs
{
    public static class MainConfigs
    {
        //Urls
        public const string Url = "http://localhost:55620/";
        public const string SignalrUrl= "http://localhost:50500/signalr";
        public const string SignalrHubName = "SocialNetHub";

        //Session keys
        public const string SessionToken = "SessionToken";
        public const string SessionFirstnameKey = "FirstnameKey";
        public const string SessionLastnameKey = "LastnameKey";
        public const string SessionUsernameKey = "UsernameKey";
        public const string SessionConnectedKey = "ConnectedKey";
        //Facebook Auth
        public const string FacebookAppId = "2205509553038336";
        public const string FacebookSecretKey = "c9905f631124307e002d2f07899f48f3";

       
    }
}