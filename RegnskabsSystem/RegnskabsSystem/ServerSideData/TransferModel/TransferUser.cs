using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;

namespace ServerSideData.TransferModel
{
    public class TransferUser
    {
        public TransferUser(string username, string hashPassword, string firstname, string lastname, string mail, TransferPermissions permissions)
        {
            this.username = username;
            this.hashPassword = hashPassword;
            this.firstname = firstname;
            this.lastname = lastname;
            this.mail = mail;
            this.permissions = permissions;
        }
        public TransferUser(User user, Permissions permissions)
        {
            this.username = user.username;
            this.hashPassword = user.hashPassword;
            this.firstname = user.firstname;
            this.lastname = user.lastname;
            this.mail = user.mail;
            this.permissions = new TransferPermissions(permissions);
        }

        public string username { get; }
        public string hashPassword { get; }
        public string firstname { get; }
        public string lastname { get; }
        public string mail { get;}
        public TransferPermissions permissions { get;}
    }
}
