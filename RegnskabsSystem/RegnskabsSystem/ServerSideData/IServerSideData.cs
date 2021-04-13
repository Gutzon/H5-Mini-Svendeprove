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
        public bool Logout(Validation validate);
        public Permissions GetPermissions(Validation validate);
        public bool CreateUser(Validation validate, User user);
        public bool EditUser(Validation validate, User user, User newuser);
        public bool DeleteUser(Validation validate, User user);
        public bool CreateMember(Validation validate, Member member);
        public bool EditMember(Validation validate, Member member, Member newmember);
        public bool DeleteMember(Validation validate, Member member);
        public bool ValidateTokken(Validation validate);
        public List<User> GetUsers(Validation validate, string searchvalue = "", string searchtype = "");
        public List<Member> GetMembers(Validation validate, string searchvalue = "", string searchtype = "");
        public List<FinanceEntry> GetFinances(Validation validate, string konti, string searchvalue = "", string searchtype = "");
    }
}
