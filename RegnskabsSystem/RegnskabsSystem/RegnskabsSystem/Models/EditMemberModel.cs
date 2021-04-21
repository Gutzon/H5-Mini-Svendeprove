using ServerSideData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Models
{
    public class EditMemberModel
    {
        public Member oldMember { get; set; }
        public Member newMember { get; set; }
    }
}
