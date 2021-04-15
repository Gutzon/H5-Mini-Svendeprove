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
    public class InventoryController : Controller
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IServerSideData serverSideData;
        private Random _random = new Random();

        public InventoryController(IServerSideData serverSideData, ILogger<InventoryController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }

        // User view
        public ActionResult Index()
        {
            return View();
        }


        // TODO:
        /*
        // GET: overview of inventory
        [HttpGet("overview")]
        public ActionResult<bool> Overview()
        {
            return null;
        }
        */
    }
}
