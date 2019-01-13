using SocialNetworkClient.Models;
using SocialNetworkClient.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Contracts
{
    public interface IMainModel
    {
        User LoggedInUser { get; set; }
        EditPassword EditPassword { get; set; }
    }
}