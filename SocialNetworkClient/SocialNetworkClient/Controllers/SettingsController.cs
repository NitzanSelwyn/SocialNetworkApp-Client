﻿using Newtonsoft.Json.Linq;
using SocialNetworkClient.Configs;
using SocialNetworkClient.Containers;
using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkClient.Controllers
{
    public class SettingsController : Controller
    {
        public IMainModel mainModel { get; set; }
        public IHttpClient httpClient { get; set; }
        public IInputsValidator inputsValidator { get; set; }
        public SettingsController()
        {
            mainModel = ClientContainer.container.GetInstance<IMainModel>();
            httpClient = ClientContainer.container.GetInstance<IHttpClient>();
            inputsValidator = ClientContainer.container.GetInstance<IInputsValidator>();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditSettings()
        {
            //returns the user's settings view
            return View("UserSettings", mainModel);
        }
        [HttpGet]
        public ActionResult ShowEditDetails(MainModel model)
        {
            //edits the details of the user (except username and password)
            if (!IsTokenValid())
            {
                return UnvalidTokenRoute();
            }
            else
            {
                return View("EditDetails", mainModel);
            }
        }
        [HttpPost]
        public ActionResult EditDetails(MainModel model)
        {
            //edits the details of the user (except username and password)
            if (!IsTokenValid())
            {
                return UnvalidTokenRoute();
            }
            else
            {
                if (FeildsAreValid(model))
                {
                    Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.EditUserDetailsRoute, model.LoggedInUser);
                    if (returnTuple.Item2 == HttpStatusCode.OK)
                    {
                        JObject jobj = new JObject();
                        jobj = (JObject)returnTuple.Item1;
                        mainModel.LoggedInUser = jobj.ToObject<User>();
                        ViewBag.SuccessMessage = "Details Updated Succesfuly";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "An Error has occurred";
                    }
                    return View("UserSettings", mainModel);
                }
                return View("EditDetails", mainModel);
            }
        }

        private bool FeildsAreValid(MainModel model)
        {
            //validates the fields and add the errors messages in to the View bag
            List<string> info = new List<string>();
            info.Add(inputsValidator.ValidateStrInput("Firstname", model.LoggedInUser.FirstName, InputsConfigs.MinGenLen, InputsConfigs.MaxGenLen));
            info.Add(inputsValidator.ValidateStrInput("Lastname", model.LoggedInUser.LastName, InputsConfigs.MinGenLen, InputsConfigs.MaxGenLen));
            info.Add(inputsValidator.ValidateRegEx("Email", InputsConfigs.EmailRegEx, model.LoggedInUser.Email));
            info.Add(inputsValidator.ValidateDateInput("Birthdate", model.LoggedInUser.BirthDate));
            info.Add(inputsValidator.ValidateStrInput("Address", model.LoggedInUser.Address, InputsConfigs.MinGenLen, InputsConfigs.MaxGenLen));
            info.Add(inputsValidator.ValidateStrInput("Work Location", model.LoggedInUser.WorkLocation, InputsConfigs.MinGenLen, InputsConfigs.MaxGenLen));
            List<string> errors = info.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            if (errors.Count == 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    ViewBag[$"Error{i}"] = errors[i];
                }
                return false;
            }

        }

        private ActionResult UnvalidTokenRoute()
        {
            //returns the user to the main window, with a pop message of logged out and clear the session data
            Session[MainConfigs.SessionFirstnameToken] = null;
            Session[MainConfigs.SessionLastnameToken] = null;
            Session[MainConfigs.SessionToken] = null;
            ViewBag.ErrorMessag = "Session Timeout, Logged out of the system";
            return View("~/Views/Home/Index.cshtml");


        }
        public bool IsTokenValid()
        {
            //validates the token from the session
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.ValidateTokenRoute, Session[MainConfigs.SessionToken]);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return Convert.ToBoolean(returnTuple.Item1);
            }
            else
            {
                return false;
            }
        }
        private User GetMyUser()
        {
            //returns the user from this token
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.GetMyUserRoute, Session[MainConfigs.SessionToken]);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JObject jobj = new JObject();
                jobj = (JObject)returnTuple.Item1;
                return jobj.ToObject<User>();
            }
            else
            {
                return null;
            }

        }
        private void SaveDetailsToSession(User user)
        {
            //saves the user's first and last name to the session for visualisation
            Session[MainConfigs.SessionFirstnameToken] = user.FirstName;
            Session[MainConfigs.SessionLastnameToken] = user.LastName;
        }
    }
}