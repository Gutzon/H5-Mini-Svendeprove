﻿using Microsoft.AspNetCore.Http;
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

        [HttpGet("creation")]
        public ActionResult Creation()
        {
            return View();
        }

        [HttpGet("edit")]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpGet("deletion")]
        public ActionResult Deletion()
        {
            return View();
        }


        [HttpPost("login")]
        public ActionResult<UserLogin> Login([FromBody] LoginModel loginData)
        {
            var hashedPassword = SecurityHelper.GetHashCode(loginData.user + loginData.GetUnEscapedPassword);
            var userLogin = serverSideData.Login(loginData.user, hashedPassword);
            switch (userLogin.status)
            {
                case "Error":
                    return BadRequest("AccessDenied");
                case "Fail":
                    return BadRequest("AccountFail");
                default:

                    // Todo: Ask Kennie to extend so login returns the edit/delete rights or perhaps all of the users rights
                    // or allow user to get their own user rights using get permissions... for now use workaround
                    var adminLogin = serverSideData.Login("admin", SecurityHelper.GetHashCode("admin"+"admin"));
                    var users = serverSideData.GetUsers(new Validation("admin", adminLogin.tokken));
                    var currentUserRights = users.FirstOrDefault(u => u.username == loginData.user).permissions;
                    userLogin.editRights = currentUserRights.EditUser;
                    userLogin.deleteRights = currentUserRights.DeleteUser;
                    return Ok(userLogin);
            };
        }


        [HttpPost("logout")]
        public ActionResult<bool> LogOut()
        {
            var validation = CookieHelper.GetValidation(Request);
            if (validation == null) return true;
            return serverSideData.Logout(validation);
        }


        // GET: overview (gets user list)
        [HttpGet("overview")]
        public ActionResult<IEnumerable<TransferUser>> Overview()
        {
            var validation = CookieHelper.GetValidation(Request);
            var users = serverSideData.GetUsers(validation);
            
            // When sending js data back on user overview - then exclude password hashes or permissions
            foreach (var user in users)
            {
                user.hashPassword = "";
                user.permissions = null;
            }
            return Ok(users);
        }



        #region UserCreation
        private string GetRandomPassword(int length = 11)
        {
            var passBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                passBuilder.Append((char)_random.Next(33, 126));
            }
            return passBuilder.ToString();
        }

        [HttpPost()]
        public ActionResult<UserCreatedModel> CreateUser([FromBody] TransferUser user)
        {
            var validation = CookieHelper.GetValidation(Request);
            if (validation == null) return new UserCreatedModel(false, "FailSession");

            var newPassword = GetRandomPassword();
            user.hashPassword = SecurityHelper.GetHashCode(user.username + newPassword);

            var creationMsg = serverSideData.CreateUser(validation, user);
            var userCreated = IsUserCreated(creationMsg, out var errorType);

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
