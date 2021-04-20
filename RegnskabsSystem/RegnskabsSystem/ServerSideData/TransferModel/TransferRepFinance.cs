using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;

namespace ServerSideData.TransferModel
{
    public class TransferRepFinance
    {
        public TransferRepFinance()
        {
        }

        public TransferRepFinance(RepFinanceEntry entry, Konti konti, string type = "" )
        {
            if (type == "Full")
            {
                ID = entry.ID;
                this.comment = entry.comment;
                this.byWho = entry.byWho;
            }
            this.value = entry.value;
            this.konti = konti.name;
            this.intervalType = entry.intervalType;
            this.intervalValue = entry.intervalValue;
            this.nextExecDate = entry.nextExecDate;
        }


        public int ID { get; set; }
        public string konti { get; set; }
        public double value { get; set; }
        public string comment { get; set; }
        public string byWho { get; set; }
        public string intervalType { get; set; }
        public int intervalValue { get; set; }
        public DateTime nextExecDate { get; set; }
    }
}
