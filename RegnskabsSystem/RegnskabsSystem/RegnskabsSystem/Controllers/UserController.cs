using Microsoft.AspNetCore.Http;
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
using System.Threading.Tasks;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IServerSideData serverSideData;
        private Random _random = new Random();

        public UserController(IServerSideData serverSideData, ILogger<UserController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }

        // User view
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost("login")]
        public ActionResult<UserLogin> Login([FromBody] LoginModel loginData)
        {
            var hashedPassword = SecurityHelper.GetHashCode(loginData.user + loginData.GetUnEscapedPassword);
            var userLogin = serverSideData.Login(loginData.user, hashedPassword);
            return userLogin.status switch
            {
                "Error" => BadRequest("AccessDenied"),
                "Fail" => BadRequest("AccountFail"),
                _ => Ok(userLogin),
            };
        }


        [HttpPost("logout")]
        public ActionResult<bool> LogOut()
        {
            var validation = CookieHelper.GetValidation(Request);
            if (validation == null) return true;
            return serverSideData.Logout(validation);
        }



        // TODO:
        /*
        // GET: overview (gets user list)
        [HttpGet("overview")]
        public ActionResult<bool> Overview()
        {
            serverSideData.GetUsers();
            return null;
        }
        */


        private string GetRandomPassword(int length = 11)
        {
            var passBuilder = new StringBuilder();
            for(var i = 0; i < length; i++)
            {
                passBuilder.Append((char)_random.Next(33, 126));
            }
            return passBuilder.ToString();
        }

        // POST: user (creates a new user)
        [HttpPost()]
        public ActionResult<bool> Post([FromBody] User user)
        {
            var validation = CookieHelper.GetValidation(Request);
            if (validation == null) return false;

            var newPassword = GetRandomPassword();
            user.hashPassword = SecurityHelper.GetHashCode(user.username + newPassword);

            var userCreated = serverSideData.CreateUser(validation, user);
            if (userCreated)
            {
                // Send e-mail til bruger med adgangskoden...
            }
            return Ok(userCreated);
        }

        /* TODO
        // GET: user/2 (gets specific user)
        [HttpGet("{userId}")]
        public ActionResult<bool> Get(int userId)
        {
            serverSideData.GetUsers("searchId/userName?");
            return null;
        }

        // POST: user/2 (edits user)
        [HttpPost("{userId}")]
        public ActionResult<bool> Post(int userId, [FromBody] IFormCollection collection)
        {
            serverSideData.EditUser();
            return null;
        }

        // POST: user/2/delete (edits user)
        [HttpPost("{userId}/delete")]
        public ActionResult<bool> Delete(int userId, [FromBody]IFormCollection collection)
        {
            serverSideData.DeleteUser();
            return null;
        }*/
    }
}
