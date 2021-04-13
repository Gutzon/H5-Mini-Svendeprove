using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        public int CorporationID { get; set; }
        public string itemName { get; set; }
        public int value { get; set; }
    }
}
