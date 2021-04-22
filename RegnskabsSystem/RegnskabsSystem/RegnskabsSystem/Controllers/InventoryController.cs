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
            /* Temp return data - deactivate when ready */
            var tempReturn = new List<Inventory>()
            {
                new Inventory()
                {
                    ID = 1,
                    itemName = "Låne håndklæde",
                    value = 20
                },

                new Inventory()
                {
                    ID = 2,
                    itemName = "Pølsebrød",
                    value = 20
                },

                new Inventory()
                {
                    ID = 3,
                    itemName = "Computer",
                    value = 1000
                },

                new Inventory()
                {
                    ID = 4,
                    itemName = "Træningsvest",
                    value = 30
                },

                new Inventory()
                {
                    ID = 5,
                    itemName = "Plæneklipper",
                    value = 750
                }
            };

            return Ok(tempReturn);

            var inventoryList = serverSideData.GetInven(Credentials);
            return Ok(inventoryList);

        }

        [HttpPost()]
        public ActionResult<IEnumerable<Inventory>> CreateInventory([FromBody] Inventory inventory)
        {
            var inventoryAddSuccess = serverSideData.CreateInven(Credentials, inventory);
            return Ok(inventoryAddSuccess);
        }

        [HttpPost()]
        public ActionResult<string> DeleteInventory([FromBody] Inventory inventory)
        {
            var inventoryAddSuccess = serverSideData.DeleteInven(Credentials, inventory);
            return Ok(inventoryAddSuccess);
        }

        [HttpPost()]
        public ActionResult<string> EditInventory([FromBody] EditInventoryModel inventory)
        {
            var inventoryAddSuccess = serverSideData.EditInven(Credentials, inventory.oldInventory, inventory.newInventory);
            return Ok(inventoryAddSuccess);
        }

        #endregion
    }
}
