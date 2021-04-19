using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
using ServerSideData.Models;
using ServerSideData.TransferModel;
using System.Collections.Generic;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        #region Attributes and constructors
        private readonly ILogger<AccountController> _logger;
        private readonly IServerSideData serverSideData;
        private Validation Credentials => CookieHelper.GetValidation(Request);

        public AccountController(IServerSideData serverSideData, ILogger<AccountController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }
        #endregion

        #region View pages (Raw HTML delivered)
        public IActionResult Index() => View();
        #endregion

        #region Api -> Serverside queries
        [HttpGet("accounts")]
        public ActionResult<IEnumerable<string>> Accounts()
        {
            return Ok(serverSideData.GetKonties(Credentials));
        }

        [HttpPost()]
        public ActionResult<string> MakeFinanceEntry([FromBody] NewAccountModel newAccount)
        {
            return Ok(serverSideData.AddKonti(Credentials, newAccount.AccountName));
        }

        [HttpPost("finance")]
        public ActionResult<string> MakeFinanceEntry([FromBody] TransferFinance newFinanceEntry)
        {
            return Ok(serverSideData.AddFinance(Credentials, newFinanceEntry));
        }

        [HttpGet("overview")]
        public ActionResult<IEnumerable<TransferFinance>> Overview()
        {
            return Ok(serverSideData.GetFinances(Credentials));
        }
        #endregion
    }
}