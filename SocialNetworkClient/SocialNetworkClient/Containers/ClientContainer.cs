using SimpleInjector;
using SimpleInjector.Lifestyles;
using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models;
using SocialNetworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Containers
{
    public static class ClientContainer
    {
        public static readonly Container container;
         static ClientContainer()
        {
            if (container == null)
            {
                container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Register<IMainModel, MainModel>(Lifestyle.Singleton);
                container.Register<IHttpClient, HttpClientSender>(Lifestyle.Singleton);
            }

        }
    }
}