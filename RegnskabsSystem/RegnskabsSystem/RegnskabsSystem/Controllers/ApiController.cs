using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ServerSideData.Models;
using ServerSideData;

namespace RegnskabsSystem.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger<ApiController> _logger;
        private readonly IServerSideData serverSideData;

        public ApiController(IServerSideData serverSideData, ILogger<ApiController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
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
        public ActionResult<string> Login([FromBody] LoginModel loginData)
        {
            var hashedPassword = SecurityHelper.GetHashCode(loginData.user + loginData.password);
            var tokenString = serverSideData.Login(loginData.user, loginData.password);
            if (!string.IsNullOrEmpty(tokenString) && !tokenString.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                return Ok(tokenString);
            }

            return BadRequest("Access to system was not granted");
        }
    }
}
