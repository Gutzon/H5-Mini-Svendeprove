using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class RepFinanceEntry
    {
        public int ID { get; set; }
        public int KontiID { get; set; }
        public double value { get; set; }
        public string comment { get; set; }
        public string byWho { get; set; }
        public string intervalType { get; set; }
        public int intervalValue { get; set; }
        public DateTime firstExecDate { get; set; }
        public DateTime nextExecDate { get; set; }
    }
}
