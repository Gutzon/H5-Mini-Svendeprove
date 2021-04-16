using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.TransferModel;

namespace ServerSideData.Models
{
    public class User
    {
        public User()
        {
        }
        public User(TransferUser user)
        {
            this.username = user.username;
            this.hashPassword = user.hashPassword;
            this.firstname = user.firstname;
            this.lastname = user.lastname;
            this.mail = user.mail;
        }
        public User(Session ses)
        {
            this.username = ses.username;
        }

        public User(User user, TransferUser tuser)
        {
            Id = user.Id;
            this.username = user.username;
            if (tuser.hashPassword != "" && tuser.hashPassword != null)
            {
                this.hashPassword = hashPassword;
            }
            this.firstname = tuser.firstname;
            this.lastname = tuser.lastname;
            this.mail = tuser.mail;
            lastSeen = user.lastSeen;
        }

        public int Id { get; set; }
        public string username { get; set; }
        public string hashPassword { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mail { get; set; }
        public DateTime lastSeen { get; set; }

    }
}
