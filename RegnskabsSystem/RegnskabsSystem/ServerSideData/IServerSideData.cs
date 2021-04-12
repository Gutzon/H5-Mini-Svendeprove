using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;


namespace ServerSideData
{
    public interface IServerSideData
    {
        public string Login(string username, string password);
        public bool Logout(string tokken);
        public Permissions GetPermissions(string tokken);
        public bool CreateUser(string tokken, User user);
        public bool EditUser(string tokken, User user);
        public bool DeleteUser(string tokken, User user);
        public bool CreateMember(string tokken, Member member);
        public bool EditMember(string tokken, Member member);
        public bool DeleteMember(string tokken, Member member);
        public List<FinanceEntry> GetFinans(string tokken, string konti, string searchvalue = "", string searchtype = "");
    }
}
