using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class Session
    {
        public Session(string username, int userId, string tokken, DateTime lastUsed, Permissions permissions)
        {
            this.username = username;
            this.userId = userId;
            this.tokken = tokken;
            this.lastUsed = lastUsed;
            this.permissions = permissions;
        }

        public string username { get;}
        public int userId { get;}
        public string tokken { get;}
        public DateTime lastUsed { get; }
        public Permissions permissions { get; }
    }
}
