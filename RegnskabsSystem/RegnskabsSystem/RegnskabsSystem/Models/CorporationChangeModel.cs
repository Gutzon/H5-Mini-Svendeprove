using ServerSideData.TransferModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Models
{
    public class CorporationChangeModel
    {
        public bool ChangeSuccess { get; set; }
        public TransferPermissions permissions {get; set; }
    }
}
