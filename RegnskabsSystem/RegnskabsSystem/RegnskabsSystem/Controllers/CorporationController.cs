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
    public class CorporationController : Controller
    {
        private readonly IServerSideData serverSideData;
        private readonly ILogger<CorporationController> logger;

        public CorporationController(IServerSideData serverSideData, ILogger<CorporationController> logger)
        {
            this.serverSideData = serverSideData;
            this.logger = logger;
        }

        [HttpPost("change")]
        public CorporationChangeModel ChangeCorporation([FromBody]Dictionary<string, int> corporationSelectionData)
        {
            var corporationSelectionId = corporationSelectionData.First().Value;
            var validation = CookieHelper.GetValidation(Request);

            
            var corporationSuccess = serverSideData.SelectCorporation(validation, corporationSelectionId);
            var corporationChangeModel = new CorporationChangeModel() {
                ChangeSuccess = corporationSuccess
            };
            if (corporationSuccess)
            {
                var users = serverSideData.GetUsers(validation);
                var currentUser = users.FirstOrDefault(u => u.username == validation.username);
                corporationChangeModel.permissions = currentUser.permissions;
            }

            return corporationChangeModel;

        }
    }
}
