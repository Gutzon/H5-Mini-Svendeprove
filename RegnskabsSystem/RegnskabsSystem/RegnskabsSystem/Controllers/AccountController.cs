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
            var accounts = serverSideData.GetKonties(Credentials);

            // Main account must always be first in list for select option
            if(accounts.Any(a => a == "Main")){
                var accountList = accounts.ToList();
                accountList.Remove("Main");
                accountList.Insert(0, "Main");
                accounts = accountList.AsEnumerable<string>();
            }

            return Ok(accounts);
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

        [HttpPost("overview")]
        public ActionResult<IEnumerable<TransferFinance>> Overview([FromBody] NewAccountModel accountToUse)
        {
            if (accountToUse.AccountName == null) accountToUse.AccountName = "";
            return Ok(serverSideData.GetFinances(Credentials, accountToUse.AccountName));
        }

        [HttpPost("change")]
        public ActionResult<string> Change([FromBody] NewAccountModel newAccountName)
        {
            if (newAccountName.AccountName == "Main") return BadRequest("ErrorMainAccount");

            var accountChangeSuccess = serverSideData.ChangeKontiName(Credentials, newAccountName.AccountName, newAccountName.NewAccountName);

            if (accountChangeSuccess == "OK") return Ok(accountChangeSuccess);
            else return BadRequest(accountChangeSuccess);
        }


        [HttpPost("finance/repeated")]
        public ActionResult<string> MakeFinanceEntryRepeated([FromBody] TransferRepFinance repeatFinance)
        {
            var repeatFinanceSuccess = serverSideData.AddRepFinance(Credentials, repeatFinance);
            return Ok(repeatFinanceSuccess);
        }


        [HttpPost("finance/repeated/delete")]
        public ActionResult<string> DeleteFinanceEntryRepeated([FromBody] TransferRepFinance repeatFinance)
        {
            var repeatFinanceSuccess = serverSideData.RemoveRepFinance(Credentials, repeatFinance);
            return Ok(repeatFinanceSuccess);
        }


        [HttpGet("finance/repeated")]
        public ActionResult<IEnumerable<TransferRepFinance>> GetFinanceEntriesRepeated()
        {
            var repeatFinanceSuccess = serverSideData.GetRepFinance(Credentials);
            return Ok(repeatFinanceSuccess);
        }
        #endregion
    }
}