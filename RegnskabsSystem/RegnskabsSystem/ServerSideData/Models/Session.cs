using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class Session
    {
        public Session(string username, int userId, string tokken, DateTime lastUsed)
        {
            this.username = username;
            this.userId = userId;
            this.tokken = tokken;
            this.lastUsed = lastUsed;
        }

        public string username { get;}
        public int userId { get;}
        public int corporationId { get; set; }
        public string tokken { get;}
        public DateTime lastUsed { get; set; }
        public Permissions permissions { get; set; }
    }
}
