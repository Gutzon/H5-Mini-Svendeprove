using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
using ServerSideData.Models;
using ServerSideData.TransferModel;
using System;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        #region Attributes and constructors
        private readonly ILogger<InventoryController> _logger;
        private readonly IServerSideData serverSideData;
        private Validation Credentials => CookieHelper.GetValidation(Request);

        public InventoryController(IServerSideData serverSideData, ILogger<InventoryController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }

        #endregion

        #region View pages (Raw HTML delivered)
        public ActionResult Index() => View();
        #endregion

        #region Api -> Serverside queries
        // TODO:
        /*
        // GET: overview of inventory
        [HttpGet("overview")]
        public ActionResult<bool> Overview()
        {
            return null;
        }
        */
        #endregion
    }
}
