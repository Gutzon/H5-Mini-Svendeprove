using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
using ServerSideData.Models;
using System.Collections.Generic;
using System.Linq;
namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class CorporationController : Controller
    {
        #region Attributes and constructors
        private readonly IServerSideData serverSideData;
        private readonly ILogger<CorporationController> logger;
        private Validation Credentials => CookieHelper.GetValidation(Request);

        public CorporationController(IServerSideData serverSideData, ILogger<CorporationController> logger)
        {
            this.serverSideData = serverSideData;
            this.logger = logger;
        }
        #endregion

        #region Api -> Serverside queries
        [HttpPost("change")]
        public CorporationChangeModel ChangeCorporation([FromBody]Dictionary<string, int> corporationSelectionData)
        {
            var credentials = Credentials;
            var corporationSelection = corporationSelectionData.First().Value;            
            var corporationSuccess = serverSideData.SelectCorporation(credentials, corporationSelection);
            var corporationChangeModel = new CorporationChangeModel() {
                ChangeSuccess = corporationSuccess
            };
            if (corporationSuccess)
            {
                var users = serverSideData.GetUsers(credentials);
                var currentUser = users.FirstOrDefault(u => u.username == credentials.username);
                corporationChangeModel.Permissions = currentUser.permissions;
            }

            return corporationChangeModel;

        }
        #endregion
    }
}
