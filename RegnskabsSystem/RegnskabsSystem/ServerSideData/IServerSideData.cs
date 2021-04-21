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
        public string CreateUser(Validation validate, TransferUser user);
        public bool EditUser(Validation validate, TransferUser user, TransferUser newuser);
        public string DeleteUser(Validation validate, TransferUser user);
        public string CreateMember(Validation validate, Member member);
        public string EditMember(Validation validate, Member member, Member newmember);
        public string DeleteMember(Validation validate, Member member);
        public bool ValidateTokken(Validation validate);
        public IEnumerable<TransferUser> GetUsers(Validation validate, string searchvalue = "", string searchtype = "");
        public IEnumerable<Member> GetMembers(Validation validate, string searchvalue = "", string searchtype = "");
        public string AddFinance(Validation validate, TransferFinance financeIn);
        public string AddKonti(Validation validate, string name);
        public string AddRepFinance(Validation validate, TransferRepFinance transferRepFinance);
        public string RemoveRepFinance(Validation validate, TransferRepFinance transferRepFinance);
        public IEnumerable<TransferRepFinance> GetRepFinance(Validation validate, string konti = "");
        public string ChangeKontiName(Validation validate, string oldname, string newname);
        public IEnumerable<TransferFinance> GetFinances(Validation validate, string konti = "", string searchvalue = "", string searchtype = "");
        public IEnumerable<Corporation> GetCorporations();
        public IEnumerable<string> GetKonties(Validation validate);
        public string CreateInven(Validation validate, Inventory item);
        public string EditInven(Validation validate, Inventory olditem, Inventory newitem);
        public string DeleteInven(Validation validate, Inventory item);
    }
}
