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
        [HttpGet("overview")]
        public ActionResult<IEnumerable<Inventory>> Overview()
        {
            var inventoryList = serverSideData.GetInven(Credentials);
            return Ok(inventoryList);
        }

        [HttpPost()]
        public ActionResult<IEnumerable<Inventory>> CreateInventory([FromBody] Inventory inventory)
        {
            var inventoryAddSuccess = serverSideData.CreateInven(Credentials, inventory);
            return Ok(inventoryAddSuccess);
        }

        [HttpPost("delete")]
        public ActionResult<string> DeleteInventory([FromBody] Inventory inventory)
        {
            var inventoryAddSuccess = serverSideData.DeleteInven(Credentials, inventory);
            return Ok(inventoryAddSuccess);
        }

        [HttpPost("edit")]
        public ActionResult<string> EditInventory([FromBody] EditInventoryModel inventory)
        {
            inventory.newInventory.CorporationID = inventory.oldInventory.CorporationID;
            var inventoryAddSuccess = serverSideData.EditInven(Credentials, inventory.oldInventory, inventory.newInventory);
            return Ok(inventoryAddSuccess);
        }

        #endregion
    }
}
