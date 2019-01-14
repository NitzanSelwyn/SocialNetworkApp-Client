using Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialNetworkClient.Configs;
using SocialNetworkClient.Containers;
using SocialNetworkClient.Contracts;
using SocialNetworkClient.Models;
using SocialNetworkClient.Models.Posts;
using SocialNetworkClient.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkClient.Controllers
{
    public class HomeController : Controller
    {
        public IMainModel mainModel { get; set; }
        public IHttpClient httpClient { get; set; }

        public HomeController()
        {
            mainModel = ClientContainer.container.GetInstance<IMainModel>();
            httpClient = ClientContainer.container.GetInstance<IHttpClient>();
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallBack");
                return uriBuilder.Uri;
            }
        }

        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = MainConfigs.FacebookAppId,
                client_secret = MainConfigs.FacebookSecretKey,
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);

        }

        public ActionResult FacebookCallBack(string code)
        {
            var fb = new FacebookClient();
            dynamic facebookResult = fb.Post("oauth/access_token", new
            {
                client_id = "2205509553038336",
                client_secret = "c9905f631124307e002d2f07899f48f3",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = facebookResult.access_token;
            Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;
            dynamic me = fb.Get("me?fields=link,first_name,last_name,email,id");
            string email = me.email;
            string firstName = me.first_name;
            string lastName = me.last_name;
            string userId = me.id;

            FacebookUser facebookUser = new FacebookUser(userId, firstName, lastName);
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.FacebookLoginRoute, facebookUser);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JObject jobj = new JObject();
                jobj = (JObject)returnTuple.Item1;
                LoginRegisterResponse obj = jobj.ToObject<LoginRegisterResponse>();
                mainModel.LoggedInUser = obj.user;
                SaveDetailsToSession(obj.user);
                SaveTokenToSession(obj.token);
                return View("Index", mainModel);

            }
            ViewBag.ErrorMessage = "An error has occurred.";

            return View("Index", mainModel);
        }

        public ActionResult Index()
        {
            if (IsTokenValid())
            {
                User user = GetMyUser();
                List<Post> pl = GetPosts(ApiConfigs.GetFollowingPosts, user.Username);
                mainModel.PostList = pl;

                return View("index", mainModel);
            }
            return View("index", mainModel);
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
        public ActionResult SearchUsers(MainModel model)
        {
            //searches for a user
            if (IsTokenValid())
            {
                Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.SearchUsersRoute, model.SearchInput);
                if (returnTuple.Item2 == HttpStatusCode.OK)
                {
                    JArray jarr = new JArray();
                    jarr = (JArray)returnTuple.Item1;
                    if (jarr != null)
                    {
                        model.SearchedUsers = jarr.ToObject<List<User>>();
                    }
                    return View("Index", model);
                }
                else
                {
                    ViewBag.ErrorMessage = "An error has occurred.";
                    return View("Index", model);
                }
            }
            else
            {
                return UnvalidTokenRoute();
            }

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
            Session[MainConfigs.SessionUsernameToken] = user.Username;
        }

        public ActionResult SendPost(MainModel model)
        {
            if (IsTokenValid())
            {
                model.LoggedInUser = GetMyUser();
                if (model.LoggedInUser != null)
                {
                    //sends a new post
                    Post post = new Post();
                    post.Author = model.LoggedInUser.Username;
                    post.Content = model.Post.Text;
                    post.Image = CovertToByteArray(model.Post.Image);
                    post.FullName = $"{model.LoggedInUser.FirstName} {model.LoggedInUser.LastName}";

                    //Post newPost = new Post(model.LoggedInUser.Username,
                    //    model.Post.Text, 0, model.Post.Image.FileName);

                    Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.PostNewMessage, post);
                    if (returnTuple.Item2 == HttpStatusCode.OK)
                    {
                        return View("Index", model);
                    }
                }
            }
            return View("Index", model);

        }

        public ActionResult UserProfile()
        {
            if (IsTokenValid())
            {
                mainModel.LoggedInUser = GetMyUser();
                List<Post> pl = GetPosts(ApiConfigs.GetUsersPosts, mainModel.LoggedInUser.Username);
                mainModel.PostList = pl;
                return View("UserProfile", mainModel);
            }
            else return UnvalidTokenRoute();
        }
        public ActionResult Logout()
        {
            //logs out of the system and clears the data
            Session[MainConfigs.SessionFirstnameToken] = null;
            Session[MainConfigs.SessionLastnameToken] = null;
            Session[MainConfigs.SessionToken] = null;
            ViewBag.ErrorMessag = "Bye bye!";
            return View("Index", mainModel);
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

        private List<Post> GetPosts(string URL, string userName)
        {
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(URL, userName);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JArray jrr = new JArray();
                jrr = (JArray)returnTuple.Item1;
                return jrr.ToObject<List<Post>>();
            }
            else
            {
                return null;
            }
        }

        [OutputCache(Duration = 2000)]
        public PartialViewResult GetPostComments(Post post)
        {
            if (IsTokenValid())
            {
                post.CommentList = GetComments(post.PostId);
                return PartialView("Comments", post);
            }
            return PartialView();
        }

        private byte[] CovertToByteArray(HttpPostedFileBase fileBase)
        {
            byte[] data;
            if (fileBase != null)
            {
                using (Stream inputStream = fileBase.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                return data;
            }
            return null;      
        }

        private List<Comment> GetComments(string postId)
        {
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.GetPostsComments, postId);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JArray jrr = new JArray();
                jrr = (JArray)returnTuple.Item1;
                return jrr.ToObject<List<Comment>>();
            }
            else
            {
                return null;
            }
        }

        public ActionResult SendComment(Post post)
        {
            if (IsTokenValid())
            {
                post.NewComment.postId = post.PostId;
                post.NewComment.CommenterName = Session[MainConfigs.SessionUsernameToken].ToString();

                Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.CommentOnPost, post.NewComment);
                if (returnTuple.Item2 == HttpStatusCode.OK)
                {
                    return View("Index", mainModel);
                }
            }
            return UnvalidTokenRoute();
        }

    }

}

