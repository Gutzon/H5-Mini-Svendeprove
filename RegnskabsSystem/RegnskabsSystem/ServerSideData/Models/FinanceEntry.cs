using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class FinanceEntry
    {
        public int ID { get; set; }
        public int KontiID { get; set; }
        public double value { get; set; }
        public string comment { get; set; }
        public string byWho { get; set; }
        public DateTime addDate { get; set; }
        public DateTime payDate { get; set; }
    }
}
