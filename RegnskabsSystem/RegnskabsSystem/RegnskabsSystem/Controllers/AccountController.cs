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
using System.Threading.Tasks;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IServerSideData serverSideData;

        public AccountController(IServerSideData serverSideData, ILogger<AccountController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("accounts")]
        public ActionResult<IEnumerable<string>> Accounts()
        {
            var validation = CookieHelper.GetValidation(Request);
            var accounts = serverSideData.GetKonties(validation);
            return Ok(accounts);
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
}
}
