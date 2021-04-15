using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("view")]
        public ActionResult AccountView()
        {
            return View();
        }
        
    }
}
