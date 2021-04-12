using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RegnskabsSystem.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("user/login")]
        public ActionResult<TokenModel> Login([FromBody] LoginModel loginData)
        {
            var hashedPassword = SecurityHelper.GetHashCode(loginData.user + loginData.password);

            // Use DLL from Kennie later by sending user and password in
            var token = new TokenModel();
            token.User = loginData.user;
            token.Token = hashedPassword;
            // End todo note

            return Ok(token);
        }
    }
}
