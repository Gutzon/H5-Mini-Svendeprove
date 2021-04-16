using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;
using ServerSideData.TransferModel;

namespace ServerSideData.TransferModel
{
    public class UserLogin
    {
        public string tokken { get; set; }
        public List<Corporation> Corporations { get; set; }
        public string status { get; set; }
        public TransferUser user { get; set; }
    }
}
