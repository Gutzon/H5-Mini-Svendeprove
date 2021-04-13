using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class Validation
    {
        public Validation(string username, string tokken)
        {
            this.username = username;
            this.tokken = tokken;
        }

        public string username { get; }
        public string tokken { get; }
    }
}
