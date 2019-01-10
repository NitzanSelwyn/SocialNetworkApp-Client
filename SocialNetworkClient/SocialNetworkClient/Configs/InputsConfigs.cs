using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Configs
{
    public static class InputsConfigs
    {
        public const int MaxGenLen = 20; //the maximum len of most inputs
        public const int MinGenLen = 4;// the minimum len of most inputs
        public const int MaxEmailLen = 30;
        public const int MaxAddressLen = 35;
        public const int MaxPostTextLen = 1000;
        public const int MaxCommentTextLen = 500;
        public const string EmailRegEx = "^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$";

    }
}