using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using ServerSideData;
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
        public bool ChangeCorporation([FromBody]Dictionary<string, int> corporationSelectionData)
        {
            var corporationSelectionId = corporationSelectionData.First().Value;
            var validation = CookieHelper.GetValidation(Request);
            return serverSideData.SelectCorporation(validation, corporationSelectionId);
        }
    }
}
