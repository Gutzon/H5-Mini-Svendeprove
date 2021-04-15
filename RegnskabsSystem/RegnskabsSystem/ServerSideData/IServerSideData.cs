using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;
using ServerSideData.TransferModel;


namespace ServerSideData
{
    public interface IServerSideData
    {
        public UserLogin Login(string username, string password);
        public bool SelectCorporation(Validation validate, int ID);
        public bool Logout(Validation validate);
        public Permissions GetPermissions(Validation validate);
        public bool CreateUser(Validation validate, TransferUser user);
        public bool EditUser(Validation validate, TransferUser user, TransferUser newuser);
        public bool DeleteUser(Validation validate, TransferUser user);
        public bool CreateMember(Validation validate, Member member);
        public bool EditMember(Validation validate, Member member, Member newmember);
        public bool DeleteMember(Validation validate, Member member);
        public bool ValidateTokken(Validation validate);
        public IEnumerable<User> GetUsers(Validation validate, string searchvalue = "", string searchtype = "");
        public IEnumerable<Member> GetMembers(Validation validate, string searchvalue = "", string searchtype = "");
        public IEnumerable<FinanceEntry> GetFinances(Validation validate, string konti = "", string searchvalue = "", string searchtype = "");
        public IEnumerable<Corporation> GetCorporations();
    }
}
