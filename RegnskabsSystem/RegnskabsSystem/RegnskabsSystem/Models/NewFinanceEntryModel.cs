using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;

namespace RegnskabsSystem.Models
{
    public class NewFinanceEntryModel
    {
        public double value { get; set; }
        public string comment { get; set; }
        public string konti { get; set; }
    }
}
