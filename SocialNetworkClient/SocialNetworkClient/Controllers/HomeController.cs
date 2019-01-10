using Newtonsoft.Json.Linq;
using SocialNetworkClient.Configs;
using SocialNetworkClient.Containers;
using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models;
using SocialNetworkClient.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkClient.Controllers
{
    public class HomeController : Controller
    {
        public static List<Post> Posts = new List<Post>();
        public IMainModel mainModel { get; set; }
        public IHttpClient httpClient { get; set; }

        public HomeController()
        {
            mainModel = ClientContainer.container.GetInstance<IMainModel>();
            httpClient = ClientContainer.container.GetInstance<IHttpClient>();
        }

        public ActionResult Index()
        {
            return View(mainModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult ClientLogin(MainModel model)
        {
            //tries a client regular login
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.UserLoginRoute, model.UserLogin);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                if (returnTuple.Item1 != null)
                {
                    JObject jobj = new JObject();
                    jobj = (JObject)returnTuple.Item1;
                    LoginRegisterResponse obj = jobj.ToObject<LoginRegisterResponse>();
                    model.LoggedInUser = obj.user;
                    SaveDetailsToSession(obj.user);
                    SaveTokenToSession(obj.token);
                    return View("Index", mainModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Username or password are unvalid.";
                    return View("Index", mainModel);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "An error has occurred.";
                return View("Index", mainModel);
            }

        }

        private void SaveTokenToSession(string token)
        {
            //saves the token to the session
            Session[MainConfigs.SessionToken] = token;
        }

        [HttpGet]
        public ActionResult OpenRegister(MainModel model)
        {
            //opens the register window
            return View("Register", model);
        }

        [HttpPost]
        public ActionResult SendRegister(MainModel model)
        {
            //tries to send a user registration
            if (ModelState.IsValid)
            {
                if (model.UserRegister.Password != model.UserRegister.PasswordConfirm)
                {
                    ViewBag.ErrorMessage = "Passwords dont match";
                    return View("Register", model);
                }
                Tuple<object, HttpStatusCode> returnTuple1 = httpClient.PostRequest(ApiConfigs.UsernameExistsRoute, model.UserRegister.Username);
                if (returnTuple1.Item2 == HttpStatusCode.OK)
                {
                    bool userNameExists = Convert.ToBoolean(returnTuple1.Item1);
                    if (userNameExists)
                    {
                        ViewBag.ErrorMessage = "Username already exists";
                        return View("Register", model);
                    }
                    else
                    {
                        Tuple<object, HttpStatusCode> returnTuple2 = httpClient.PostRequest(ApiConfigs.UserRegisterRoute, model.UserRegister);
                        if (returnTuple2.Item2 == HttpStatusCode.OK)
                        {
                            JObject jobj = new JObject();
                            jobj = (JObject)returnTuple2.Item1;
                            LoginRegisterResponse response = jobj.ToObject<LoginRegisterResponse>();
                            SaveTokenToSession(response.token);
                            SaveDetailsToSession(response.user);
                            ViewBag.SuccessMessage = "Registration successful";
                            return View("Index", model);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "An Error has acquired";
                            return View("Register", model);
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "an Error has acquired";
                }
            }
            return View("Register", model);
        }

        private void SaveDetailsToSession(User user)
        {
            //saves the user's first and last name to the session for visualisation
            Session[MainConfigs.SessionFirstnameToken] = user.FirstName;
            Session[MainConfigs.SessionLastnameToken] = user.LastName;
        }

        public ActionResult SendPost(MainModel model)
        {
            if (model.LoggedInUser != null)
            {
                //sends a new post
                Post newPost = new Post(model.LoggedInUser.ToString(),
                    model.Post.Text, 0, model.Post.Image.FileName);
            }

            return View("Index", model);
        }

        public ActionResult UserProfile()
        {
            if (IsTokenValid())
            {
                mainModel.LoggedInUser = GetMyUser();
                return View("UserProfile", mainModel);
            }
            else return UnvalidTokenRoute();
        }

        private ActionResult UnvalidTokenRoute()
        {
            //returns the user to the main window, with a pop message of logged out and clear the session data
            Session[MainConfigs.SessionFirstnameToken] = null;
            Session[MainConfigs.SessionLastnameToken] = null;
            Session[MainConfigs.SessionToken] = null;
            ViewBag.ErrorMessag = "Session Timeout, Logged out of the system";
            return View("Index");


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
        

    }
}