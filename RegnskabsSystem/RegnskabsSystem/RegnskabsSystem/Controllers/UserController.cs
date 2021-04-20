using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
using ServerSideData.Models;
using ServerSideData.TransferModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        #region Attributes and constructors
        private readonly ILogger<UserController> _logger;
        private readonly IServerSideData serverSideData;
        private Validation Credentials => CookieHelper.GetValidation(Request);

        public UserController(IServerSideData serverSideData, ILogger<UserController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }
        #endregion

        #region View pages (Raw HTML delivered)
        public ActionResult Index() => View();

        [HttpGet("creation")]
        public ActionResult Creation() => View();

        [HttpGet("edit")]
        public ActionResult Edit() => View();

        [HttpGet("deletion")]
        public ActionResult Deletion() => View();

        #endregion

        #region Api -> Serverside queries
        [HttpGet("login/validate")]
        public ActionResult<bool> LoginValidation()
        {
            var loggedIn = serverSideData.ValidateTokken(Credentials);
            if (loggedIn) return Ok(loggedIn);
            return BadRequest("Session timeout / login not active");
        }

        [HttpPost("login")]
        public ActionResult<UserLogin> Login([FromBody] LoginModel loginData)
        {
            var hashedPassword = SecurityHelper.GetHashCode(loginData.User + loginData.GetUnEscapedPassword);
            // Remove clear password as soon as possible as it is a security concern
            loginData.Password = "";
            var userLogin = serverSideData.Login(loginData.User, hashedPassword);
            
            switch (userLogin.status)
            {
                case "Error":
                    return BadRequest("AccessDenied");
                case "Fail":
                    return BadRequest("AccountFail");
                default:
                    var users = serverSideData.GetUsers(new Validation(loginData.User, userLogin.tokken), "", "Self");
                    if (users == null || !users.Any())
                    {
                        return BadRequest("AccountFail");
                    }
                    userLogin.user = users.First();
                    return Ok(userLogin);
            };
        }

        [HttpPost("logout")]
        public ActionResult<bool> LogOut()
        {
            serverSideData.Logout(Credentials);
            return Ok(true);
        }

        [HttpGet("overview")]
        public ActionResult<IEnumerable<TransferUser>> Overview()
        {
            var users = serverSideData.GetUsers(Credentials);
            return Ok(users);
        }

        [HttpPost()]
        public ActionResult<UserCreatedModel> CreateUser([FromBody] TransferUser user)
        {
            var newPassword = SecurityHelper.GetRandomPassword();
            user.hashPassword = SecurityHelper.GetHashCode(user.username + newPassword);

            var creationMsg = serverSideData.CreateUser(Credentials, user);
            var userCreated = IsUserCreated(creationMsg, out var errorType);

            // Send mail to user with initial password or confirm link where user can create password
            // The password transferral to frontend is needed as we do not have a hotel for the application (no mail to user)
            var userCreatedModel = new UserCreatedModel(userCreated, errorType, newPassword);

            return Ok(userCreatedModel);
        }

        private bool IsUserCreated(string creationMsg, out string errorType)
        {
            errorType = "";
            switch (creationMsg)
            {
                case "No session":
                    errorType = "FailSession";
                    break;
                case "Failed setting permission check":
                    errorType = "FailPermission";
                    break;
                case "Not permited":
                    errorType = "FailUserAddRights";
                    break;
                case "Username already in use":
                    errorType = "FailUserExists";
                    break;
                default:
                    return true;
            }
            return false;
        }
        #endregion
    }
}