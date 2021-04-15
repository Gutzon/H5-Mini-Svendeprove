using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;

namespace ServerSideData.TransferModel
{
    public class UserLogin
    {
        public string tokken { get; set; }
        public List<Corporation> Corporations { get; set; }
        public string status { get; set; }
        public bool editRights { get; set; }
        public bool deleteRights { get; set; }
    }
}
