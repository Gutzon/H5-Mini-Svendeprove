using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;

namespace ServerSideData.TransferModel
{
    public class TransferFinance
    {
        public TransferFinance()
        {
        }

        public TransferFinance(FinanceEntry entry, Konti konti, string type = "" , bool kontifiltered = false)
        {
            if (type == "Full")
            {
                ID = entry.ID;
                this.comment = entry.comment;
                this.byWho = entry.byWho;
            }
            if (kontifiltered)
            {
                this.newSaldo = entry.newSaldoKonti;
            }
            else
            {
                this.newSaldo = entry.newSaldoMain;
            }
            this.value = entry.value;
            this.konti = konti.name;
            this.payDate = entry.payDate;
            this.addDate = entry.addDate;
        }

        public int ID { get; set; }
        public double value { get; set; }
        public string comment { get; set; }
        public string byWho { get; set; }
        public double newSaldo { get; set; }

        public DateTime addDate { get; set; }
        public DateTime payDate { get; set; }
        public string konti { get; set; }
    }
}
