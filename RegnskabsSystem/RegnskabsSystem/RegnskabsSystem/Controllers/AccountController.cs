using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
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
            var validation = CookieHelper.GetValidation(Request);
            var accounts = serverSideData.GetKonties(validation);
            return Ok(accounts);
        }

        [HttpPost()]
        public ActionResult<string> MakeFinanceEntry([FromBody] NewAccountModel newAccount)
        {
            var validation = CookieHelper.GetValidation(Request);
            var financeAddSuccess = serverSideData.AddKonti(validation, newAccount.AccountName);
            return Ok(financeAddSuccess);
        }

        [HttpPost("finance")]
        public ActionResult<string> MakeFinanceEntry([FromBody] TransferFinance newFinanceEntry)
        {
            var validation = CookieHelper.GetValidation(Request);
            var financeAddSuccess = serverSideData.AddFinance(validation, newFinanceEntry);
            return Ok(financeAddSuccess);
        }

        [HttpGet("overview")]
        public ActionResult<IEnumerable<TransferFinance>> Overview()
        {
            var validation = CookieHelper.GetValidation(Request);
            var finances = serverSideData.GetFinances(validation);
            return Ok(finances);
        }
        #endregion
    }
}