using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class Permissions
    {
        public int ID { get; set; }
        public bool AddCorporation { get; }
        public bool Admin { get; }
        public bool AddUser { get; }
        public bool EditUser { get; }
        public bool DeleteUser { get; }
        public bool AddMember { get; }
        public bool EditMember { get; }
        public bool DeleteMember { get; }
        public bool AddFinance { get; }
        public bool ViewFinance { get; }
        public bool LimitedViewFinance { get; }
        public bool AddInventory { get; }
        public bool EditInventory { get; }
        public bool DeleteInventory { get; }



    }
}
