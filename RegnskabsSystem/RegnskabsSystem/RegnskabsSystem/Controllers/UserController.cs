﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
using ServerSideData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IServerSideData serverSideData;

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
        public ActionResult<string> Login([FromBody] LoginModel loginData)
        {
            var hashedPassword = SecurityHelper.GetHashCode(loginData.user + loginData.password);
            var tokenString = serverSideData.Login(loginData.user, hashedPassword);
            var tokenLower = tokenString.ToLower();
            switch (tokenLower)
            {
                case "null":
                    return BadRequest("AccessDenied");
                case "error":
                    return BadRequest("AccountFail");
                default:
                    return Ok(tokenString);
            }
        }

        private Validation GetValidation()
        {
            // Cases where request for logout should not occur as it already is logged out.
            if (!Request.Cookies.TryGetValue("accessToken", out var accessTokenValue)
                || string.IsNullOrEmpty(accessTokenValue))
            {
                return null;
            }

            if (!Request.Cookies.TryGetValue("userName", out var userName)
                || string.IsNullOrEmpty(userName))
            {
                return null;
            }

            return new Validation(userName, accessTokenValue);
        }

        [HttpPost("logout")]
        public ActionResult<bool> LogOut()
        {
            var validation = GetValidation();
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

        // POST: user (creates a new user)
        [HttpPost()]
        public ActionResult<bool> Post([FromBody] User user)
        {
            var validation = GetValidation();
            if (validation == null) return false;

            var tempPassword = "SoNotSecret"; // Fix to be random password on user creation
            user.hashPassword = SecurityHelper.GetHashCode(user.username + tempPassword);

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