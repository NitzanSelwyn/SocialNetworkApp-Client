using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SocialNetworkClient.Contracts
{
    public interface IHttpClient
    {
        Tuple<object, HttpStatusCode> PostRequest(string route, object obj = null);
        Tuple<object, HttpStatusCode> GetRequest(string route);
    }
}