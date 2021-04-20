using ServerSideData.TransferModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegnskabsSystem.Models
{
    public class EditAccountModel
    {
        public TransferUser oldUser { get; set; }
        public TransferUser newUser { get; set; }
    }
}
