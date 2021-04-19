using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
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
        public ActionResult<IEnumerable<Konti>> Accounts()
        {
            var validation = CookieHelper.GetValidation(Request);
            //var accounts = serverSideData.GetKonti?(validation);

            var accounts = new List<Konti>();
            accounts.Add(new Konti() { CorporationID = 1, ID = 1, name = "Main" });
            accounts.Add(new Konti() { CorporationID = 1, ID = 2, name = "Salg af kioskvarer" });
            accounts.Add(new Konti() { CorporationID = 1, ID = 3, name = "Medlemsskaber" });

            return Ok(accounts);
        }


        [HttpGet("overview")]
        public ActionResult<IEnumerable<TransferFinance>> Overview()
        {
            var validation = CookieHelper.GetValidation(Request);
            var finances = serverSideData.GetFinances(validation);
            return Ok(finances);



            // Temp until endpoint ready
            /*var random = new Random();
            var finances = new List<FinanceEntry>();
            var sum = 0;
            for (var i = 0; i < 30; i++)
            {
                finances.Add(new FinanceEntry()
                {
                    value = random.Next(-500, 500),
                    addDate = new DateTime(DateTime.Now.Ticks).AddDays(random.Next(-5, -1)),
                    byWho = "User " + random.Next(1, 100),
                    comment = "Postering tekst " + random.Next(1, 10000),
                    payDate = new DateTime(DateTime.Now.Ticks).AddDays(random.Next(-5, -1)),
                    KontiID = random.Next(1,3),
                    ID = i+1
                });
            }*/


            //return Ok(finances);
        }
    }
}
