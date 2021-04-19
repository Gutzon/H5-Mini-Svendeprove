using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using System.Diagnostics;
using ServerSideData;

namespace RegnskabsSystem.Controllers
{
    public class DashboardController : Controller
    {
        #region Attributes and constructors
        private readonly ILogger<DashboardController> _logger;
        private readonly IServerSideData serverSideData;

        public DashboardController(IServerSideData serverSideData, ILogger<DashboardController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }
        #endregion

        #region View pages (Raw HTML delivered)
        public IActionResult Index() => View();
        #endregion

        #region Api -> Serverside queries

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
