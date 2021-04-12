using System;
using System.Collections.Generic;
using ServerSideData.Models;

namespace ServerSideData
{
    public class ServerSideData : IServerSideData
    {
        private readonly FinanceDbContext db;
        public ServerSideData(FinanceDbContext db)
        {
            this.db = db;
        }
        private int Commit()
        {
            return db.SaveChanges();
        }

        public bool CreateMember(string tokken, Member member)
        {
            throw new NotImplementedException();
        }

        public bool CreateUser(string tokken, User user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteMember(string tokken, Member member)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string tokken, User user)
        {
            throw new NotImplementedException();
        }

        public bool EditMember(string tokken, Member member)
        {
            throw new NotImplementedException();
        }

        public bool EditUser(string tokken, User user)
        {
            throw new NotImplementedException();
        }

        public List<FinanceEntry> GetFinans(string tokken, string konti, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public Permissions GetPermissions(string tokken)
        {
            throw new NotImplementedException();
        }

        public string Login(string username, string password)
        {
            if (username == "foo" && password == "foo")
            {
                Guid g = Guid.NewGuid();
                return g.ToString();
            }
            else
            {
                return "NULL";
            }
        }

        public bool Logout(string tokken)
        {
            throw new NotImplementedException();
        }

        List<FinanceEntry> IServerSideData.GetFinans(string tokken, string konti, string searchvalue, string searchtype)
        {
            throw new NotImplementedException();
        }
    }
}
