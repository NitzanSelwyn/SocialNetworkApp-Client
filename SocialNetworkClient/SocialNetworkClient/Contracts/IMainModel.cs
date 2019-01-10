using SocialNetworkClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkClient.Contracts
{
    public interface IMainModel
    {
        User LoggedInUser { get; set; }
    }
}