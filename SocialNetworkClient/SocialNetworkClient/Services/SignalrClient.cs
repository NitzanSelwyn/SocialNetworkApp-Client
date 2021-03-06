﻿using Microsoft.AspNet.SignalR.Client;
using SocialNetworkClient.Configs;
using SocialNetworkClient.Controllers;
using SocialNetworkClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkClient.Services
{
    public class SignalrClient
    {
        public string Url { get; set; }
        public HubConnection Connection { get; set; }
        public IHubProxy Hub { get; set; }
        public MainModel model { get; set; }
        public SignalrClient(MainModel model)
        {
            this.model = model;
            Connection = new HubConnection(MainConfigs.SignalrUrl);
            Hub = Connection.CreateHubProxy(MainConfigs.SignalrHubName);

            Hub.On("SignIn", (string name) =>
                {

                });
            Hub.On("GotNotificationsFromServer", (List<Notification> notifications) =>
            {
                model.Notifications.Clear();
                foreach (var item in notifications)
                {
                    model.Notifications.Add(item);
                }
            });
            Connection.Start().Wait();
        }
        public void Login(string username)
        {
            //notifies the server on log in
            Hub.Invoke("SignIn", username);
            Hub.Invoke("CheckForNotificationsOnLogin", username);
        }
      
    }
}