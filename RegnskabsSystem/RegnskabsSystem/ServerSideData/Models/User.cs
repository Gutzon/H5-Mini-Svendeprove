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

        public int Id { get; set; }
        public string username { get; set; }
        public string hashPassword { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mail { get; set; }

    }
}
