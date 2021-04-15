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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class MemberController : Controller
    {
        private readonly ILogger<MemberController> _logger;
        private readonly IServerSideData serverSideData;

        public MemberController(IServerSideData serverSideData, ILogger<MemberController> logger)
        {
            _logger = logger;
            this.serverSideData = serverSideData;
        }

        // User view
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet("creation")]
        public ActionResult Creation()
        {
            return View();
        }

        [HttpGet("edit")]
        public ActionResult Edit()
        {
            return View();
        }

        // TODO:
        /* Get member list
         * Edit member
         * Delete member
         * Create member
         * */

        /*
        [HttpPost()]
        public ActionResult<UserCreatedModel> CreateMember([FromBody] TransferUser user)
        {
        }*/

        /* TODO
        // GET: user/2 (gets specific user)
        [HttpGet("{userId}")]
        public ActionResult<bool> Get(int userId)
        {
            serverSideData.GetUsers("searchId/userName?");
            return null;
        }

        // POST: user/2 (edits user)
        [HttpPost("{userId}")]
        public ActionResult<bool> Post(int userId, [FromBody] IFormCollection collection)
        {
            serverSideData.EditUser();
            return null;
        }

        // POST: user/2/delete (edits user)
        [HttpPost("{userId}/delete")]
        public ActionResult<bool> Delete(int userId, [FromBody]IFormCollection collection)
        {
            serverSideData.DeleteUser();
            return null;
        }*/
    }
}
