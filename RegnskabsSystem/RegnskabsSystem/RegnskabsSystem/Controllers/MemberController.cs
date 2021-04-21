using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
using RegnskabsSystem.Models;
using ServerSideData;
using ServerSideData.Models;
using System.Collections.Generic;

namespace RegnskabsSystem.Controllers
{
    [Route("[controller]")]
    public class MemberController : Controller
    {
        #region Attributes and constructors
        private readonly ILogger<MemberController> _logger;
        private readonly IServerSideData serverSideData;
        private Validation Credentials => CookieHelper.GetValidation(Request);

        public MemberController(IServerSideData serverSideData, ILogger<MemberController> logger)
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
        public ActionResult<IEnumerable<Member>> Overview()
        {
            return Ok(serverSideData.GetMembers(Credentials));
        }

        [HttpPost()]
        public ActionResult<bool> CreateMember([FromBody] Member member)
        {
            var createMemberSuccess = serverSideData.CreateMember(Credentials, member);
            return Ok(createMemberSuccess);
        }

        [HttpPost("edit")]
        public ActionResult<bool> EditMember([FromBody] EditMemberModel editedMember)
        {
            var editMemberSuccess = serverSideData.EditMember(Credentials, editedMember.oldMember, editedMember.oldMember);
            return Ok(editMemberSuccess);
        }

        [HttpPost("delete")]
        public ActionResult<bool> DeleteMember([FromBody] Member member)
        {
            var deleteMemberSuccess = serverSideData.DeleteMember(Credentials, member);
            return Ok(deleteMemberSuccess);
        }
        #endregion
    }
}
