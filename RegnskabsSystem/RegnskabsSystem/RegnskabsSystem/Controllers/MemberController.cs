﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegnskabsSystem.Helpers;
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

        [HttpGet("creation")]
        public ActionResult Creation() => View();

        [HttpGet("edit")]
        public ActionResult Edit() => View();
        #endregion

        #region Api -> Serverside queries
        [HttpGet("overview")]
        public ActionResult<IEnumerable<Member>> Overview()
        {
            return Ok(serverSideData.GetMembers(Credentials));
        }
        #endregion
    }
}
