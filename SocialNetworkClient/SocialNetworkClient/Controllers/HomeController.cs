using Newtonsoft.Json.Linq;
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
    public class HomeController : Controller
    {
        public static List<Post> Posts = new List<Post>();
        public static IMainModel mainModel { get; set; }
        public IHttpClient httpClient { get; set; }

        public HomeController()
        {
            mainModel = ClientContainer.container.GetInstance<IMainModel>();
            httpClient = ClientContainer.container.GetInstance<IHttpClient>();
        }

        public ActionResult Index(MainModel mainModel)
        {
            if (mainModel != null)
            {
                return View(mainModel);
            }
            else
            {
                return View(mainModel);
            }
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

                    User user = jobj.ToObject<User>();
                    model.LoggedInUser = user;

                    return View("Index", model);
                }
                else
                {
                    ViewBag.ErrorMessage = "Username or password are unvalid.";
                    return View("Index", model);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "An error has occurred.";
                return View("Index", model);
            }


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

        public ActionResult UserProfile(string token)
        {
            return View("UserProfile", mainModel);
        }
    }
}