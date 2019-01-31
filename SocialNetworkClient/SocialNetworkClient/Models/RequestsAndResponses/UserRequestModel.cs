using SocialNetworkClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Models.RequestsAndResponses
{
    public class UserRequestModel
    {
        public string userId { get; set; } //the id of the user that made this request
        public string onUserId { get; set; }// the id of the user that the requester made on (Example: userId wants to block onUserId)
        public UserRequestEnum requestType { get; set; }
        public UserRequestModel(string userId, string onUserId, UserRequestEnum requestType)
        {
            this.userId = userId;
            this.onUserId = onUserId;
            this.requestType = requestType;

        }
        public UserRequestModel()
        {
        }
        public UserRequestModel(string userId, string onUserId)
        {
            this.userId = userId;
            this.onUserId = onUserId;
        }
    }
}