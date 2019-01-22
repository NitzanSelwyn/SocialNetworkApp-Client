using Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialNetworkClient.Configs;
using SocialNetworkClient.Containers;
using SocialNetworkClient.Contracts;
using SocialNetworkClient.Enums;
using SocialNetworkClient.Models;
using SocialNetworkClient.Models.Posts;
using SocialNetworkClient.Models.RequestsAndResponses;
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
        public MainModel mainModel { get; set; }
        public IHttpClient httpClient { get; set; }

        public HomeController()
        {
            mainModel = new MainModel();
            
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
                client_id = MainConfigs.FacebookAppId,
                client_secret = MainConfigs.FacebookSecretKey,
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
                return RedirectToAction("Index", mainModel);

            }
            ViewBag.ErrorMessage = "An error has occurred.";

            return View("Index", mainModel);
        }

        public ActionResult Index()
        {
            if (IsTokenValid())
            {
                User user = GetMyUser();
           //     GetPostMoel getPostMoel = new GetPostMoel { SkipNumber = numberToSkip.ToString(), UserName = user.Username}
                List<Post> pl = GetPosts(ApiConfigs.GetFollowingPosts, user.Username, mainModel.PostCounter);
               // mainModel.PostCounter += 10;
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
               
                    JObject jobj = new JObject();
                    jobj = (JObject)returnTuple.Item1;
                    LoginRegisterResponse obj = jobj.ToObject<LoginRegisterResponse>();
                    model.LoggedInUser = obj.user;
                    SaveDetailsToSession(obj.user);
                    SaveTokenToSession(obj.token);
                    return RedirectToAction("Index", mainModel);
                
            }
            else if (returnTuple.Item2 == HttpStatusCode.Conflict)
            {
                ViewBag.ErrorMessage = "Username or password are unvalid.";
                return View("Index", mainModel);
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
                        if (model.SearchedUsers.Count == 1)
                        {
                            string username = model.SearchedUsers[0].Username;
                            return RedirectToAction("ViewUser", "Home", new { username });
                        }
                        else if (model.SearchedUsers.Count == 0)
                        {
                            ViewBag.ErrorMessage = "No users found that match this input";
                            return View("Index", model);
                        }
                        else
                        {
                            return View("SearchedUsers", model);
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No users found that matches this input";
                        return View("Index", model);
                    }

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

        public bool ManageRequest(UserRequestModel request)
        {
            //manages the request: Follow, Unfollow,Friend,
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.ManageRequestRoute, request);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult BlockedUsers(MainModel model)
        {
            //views all the users that I blocked
            if (IsTokenValid())
            {
                Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.GetBlockedUsers, Session[MainConfigs.SessionUsernameKey]);
                if (returnTuple.Item2 == HttpStatusCode.OK)
                {
                    JArray jarr = new JArray();
                    jarr = (JArray)returnTuple.Item1;
                    model.UsersRep = jarr.ToObject<List<UserRepresentation>>();
                    return View("BlockedUsers", model);
                }
                else
                {
                    ViewBag.ErrorMessage = "An Error has occurred";
                    return View("Index", model);
                }
            }
            else
            {
                return UnvalidTokenRoute();
            }

        }

        public ActionResult UsersThatFollowMe()
        {
            //see the view of the users that follows me
            mainModel.UsersRep = GetUsersThatFollowsMe();
            return View(mainModel);

        }

        public ActionResult UnfollowUser(string id)
        {
            //Unfollows the selected user
            if (IsTokenValid())
            {
                UserRequestModel request = new UserRequestModel(Session[MainConfigs.SessionUsernameKey].ToString(), id, UserRequestEnum.UnFollow);
                if (ManageRequest(request))
                {
                    ViewBag.PageMessage = "User Unfollowed Successfully";
                }
                else
                {
                    ViewBag.PageMessage = "An Error has occurred";
                }
                return ViewUser(id);
            }
            else
            {
                return UnvalidTokenRoute();
            }
        }

        public ActionResult FollowUser(string id)
        {
            //Follows the selected user
            if (IsTokenValid())
            {
                if (id == Session[MainConfigs.SessionUsernameKey].ToString())
                {
                    ViewBag.PageMessage = "Dont try to follow yourself";
                    return ViewUser(id);
                }
                UserRequestModel request = new UserRequestModel(Session[MainConfigs.SessionUsernameKey].ToString(), id, UserRequestEnum.Follow);
                if (ManageRequest(request))
                {
                    ViewBag.PageMessage = "User Followed Successfully";
                }
                else
                {
                    ViewBag.PageMessage = "An Error has occurred";
                }
                return ViewUser(id);
            }
            else
            {
                return UnvalidTokenRoute();
            }
        }

        public ActionResult UsersImFollowing()
        {
            //shows the view of the users im following
            mainModel.UsersRep = GetUsersImFollowing();
            return View(mainModel);
        }

        public ActionResult UnblockUser(string id)
        {
            //unblocks the selected user
            if (IsTokenValid())
            {
                UserRequestModel request = new UserRequestModel(Session[MainConfigs.SessionUsernameKey].ToString(), id, UserRequestEnum.UnBlock);
                if (ManageRequest(request))
                {
                    ViewBag.PageMessage = "User Unblocked Successfully";
                }
                else
                {
                    ViewBag.PageMessage = "An Error has occurred";
                }
                return BlockedUsers(mainModel);
            }
            else
            {
                return UnvalidTokenRoute();
            }
        }

        public ActionResult BlockUser(string id)
        {
            //Blocks the selected user
            if (IsTokenValid())
            {
                if (id == Session[MainConfigs.SessionUsernameKey].ToString())
                {
                    ViewBag.PageMessage = "Dont try to block yourself";
                    return ViewUser(id);
                }
                UserRequestModel request = new UserRequestModel(Session[MainConfigs.SessionUsernameKey].ToString(), id, UserRequestEnum.Block);
                if (ManageRequest(request))
                {
                    ViewBag.PageMessage = "User blocked Successfully";
                }
                else
                {
                    ViewBag.PageMessage = "An Error has occurred";
                }
                return BlockedUsers(mainModel);
            }
            else
            {
                return UnvalidTokenRoute();
            }
        }

        public ActionResult ViewUser(string username)
        {
            //Views the profile and posts of this user (if not blocked by him)
            if (IsTokenValid())
            {
                User toView = GetUserByUsername(username);
                Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.BlockedByUsersRoute, new UserRequestModel(Session[MainConfigs.SessionUsernameKey].ToString(), toView.Username));
                if (returnTuple.Item2 == HttpStatusCode.OK)
                {
                    bool blockedMe = Convert.ToBoolean(returnTuple.Item1);
                    if (blockedMe)
                    {
                        return View("BlockedView");
                    }
                    else
                    {
                        bool FollowingThisUser = GetUsersImFollowing().Exists(ur => ur.Id == toView.Username);
                        UserViewModel userToView = new UserViewModel(toView.Username, $"{toView.FirstName} {toView.LastName}", FollowingThisUser);
                        mainModel.UserToView = userToView;
                      //  GetPostMoel getPostMoel = new GetPostMoel { SkipNumber = numberToSkip.ToString(),}
                        mainModel.PostList = GetPosts(ApiConfigs.GetUsersPosts, mainModel.UserToView.Username, mainModel.PostCounter);

                        return View("UserView", mainModel);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "An Error has acquired";
                    return View("Index");
                }
            }
            else
            {
                return UnvalidTokenRoute();
            }

        }

        private List<UserRepresentation> GetUsersImFollowing()
        {
            //returns the users that im following
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.GetFollowingUsers, Session[MainConfigs.SessionUsernameKey]);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JArray jarr = new JArray();
                jarr = (JArray)returnTuple.Item1;
                return jarr.ToObject<List<UserRepresentation>>();
            }
            else
            {
                return null;
            }
        }

        private List<UserRepresentation> GetUsersThatFollowsMe()
        {
            //gets the users that follows me
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.GetUsersThatFollowsMe, Session[MainConfigs.SessionUsernameKey]);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JArray jarr = new JArray();
                jarr = (JArray)returnTuple.Item1;
                return jarr.ToObject<List<UserRepresentation>>();
            }
            else
            {
                return null;
            }
        }

        public ActionResult AnotherUserView(MainModel model)
        {
            //views a user after verification that he/she/it didnt block me
            return View(model);

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
                            return RedirectToAction("Index", model);
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
            Session[MainConfigs.SessionConnectedKey] = true;
            Session[MainConfigs.SessionFirstnameKey] = user.FirstName;
            Session[MainConfigs.SessionLastnameKey] = user.LastName;
            Session[MainConfigs.SessionUsernameKey] = user.Username;
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
                    post.DatePosted = DateTime.Now;

                    //Post newPost = new Post(model.LoggedInUser.Username,
                    //    model.Post.Text, 0, model.Post.Image.FileName);

                    Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.PostNewMessage, post);
                    if (returnTuple.Item2 == HttpStatusCode.OK)
                    {
                        return View("Index", model);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "An Error has acquired";
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
                mainModel.PostList = GetPosts(ApiConfigs.GetUsersPosts, mainModel.LoggedInUser.Username, mainModel.PostCounter);
                
                
                var userViewModel = new UserViewModel();          
                mainModel.UserToView = userViewModel;
                
                return View("UserProfile", mainModel);
            }
            else return UnvalidTokenRoute();
        }

       
        public PartialViewResult LoadMoreInrofile(string modelFromProfile)
        {
            mainModel.LoggedInUser = GetMyUser();
            mainModel.PostCounter += 10;

            List<Post> pl = GetPosts(ApiConfigs.GetUsersPosts, modelFromProfile, mainModel.PostCounter);
            mainModel.PostList = new List<Post>();

            var userViewModel = new UserViewModel();
            mainModel.UserToView = userViewModel;
            mainModel.PostList = pl;
            return PartialView("Posts", mainModel);
        }

        public PartialViewResult LoadMorePosts(string userName)
        {
            mainModel.PostCounter += 10;
            mainModel.PostList = new List<Post>();
            List<Post> pl = GetPosts(ApiConfigs.GetFollowingPosts, userName, mainModel.PostCounter);
            mainModel.PostList = pl;

            var userViewModel = new UserViewModel();
            mainModel.UserToView = userViewModel;
            return PartialView("Posts", mainModel);
        }

        public ActionResult Logout()
        {
            //logs out of the system and clears the data
            string firstName = Session[MainConfigs.SessionFirstnameKey].ToString();
            Session[MainConfigs.SessionUsernameKey] = null;
            Session[MainConfigs.SessionFirstnameKey] = null;
            Session[MainConfigs.SessionLastnameKey] = null;
            Session[MainConfigs.SessionToken] = null;
            Session[MainConfigs.SessionConnectedKey] = null;
            ViewBag.ErrorMessage = $"Bye {firstName}!";
            return View("Index", mainModel);
        }

        private ActionResult UnvalidTokenRoute()
        {
            //returns the user to the main window, with a pop message of logged out and clear the session data
            Session[MainConfigs.SessionFirstnameKey] = null;
            Session[MainConfigs.SessionLastnameKey] = null;
            Session[MainConfigs.SessionToken] = null;
            ViewBag.ErrorMessage = "Session Timeout, Logged out of the system";
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

        private User GetUserByUsername(string username)
        {
            //returs the user that matches this username
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.GetUserByUsername, username);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JObject jobj = new JObject();
                jobj = (JObject)returnTuple.Item1;
                User user = jobj.ToObject<User>();
                return user;
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

        private List<Post> GetPosts(string URL, string userName,int skipCount)
        {
            Tuple<object, HttpStatusCode> returnTuple = httpClient.GetRequest($"{URL}/{userName}/{skipCount}");
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
                post.NewComment.CommenterName = Session[MainConfigs.SessionUsernameKey].ToString();
                post.NewComment.CommentedDate = DateTime.Now;

                Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.CommentOnPost, post.NewComment);
                if (returnTuple.Item2 == HttpStatusCode.OK)
                {
                    JObject jobj = new JObject();
                    jobj = (JObject)returnTuple.Item1;
                    var newPost = jobj.ToObject<Post>();

                    return View("Index", mainModel);
                }
            }
            return UnvalidTokenRoute();
        }

        public PartialViewResult LikeAPost(Post post)
        {
            post.Like = new Like();
            post.Like.postId = post.PostId;
            post.Like.UserName = Session[MainConfigs.SessionUsernameKey].ToString();

            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.Like, post.Like);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JObject jobj = new JObject();
                jobj = (JObject)returnTuple.Item1;
                var newPost = jobj.ToObject<Post>();
                return PartialView("PostDetails", newPost);
            }
         return PartialView();

        }

        public PartialViewResult UnLikeAPost(Post post)
        {
            post.Like = new Like();
            post.Like.postId = post.PostId;
            post.Like.UserName = Session[MainConfigs.SessionUsernameKey].ToString();

            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(ApiConfigs.UnLike, post.Like);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JObject jobj = new JObject();
                jobj = (JObject)returnTuple.Item1;
                var newPost = jobj.ToObject<Post>();
                return PartialView("PostDetails", newPost);
            }
            return PartialView();


        }
    }

}

