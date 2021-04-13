using System;
using System.Collections.Generic;
using ServerSideData.Models;
using System.Linq;

namespace ServerSideData
{
    public class ServerSideData : IServerSideData
    {
        private static List<Session> sessions = new List<Session>();
        private readonly FinanceDbContext db;
        public ServerSideData(FinanceDbContext db)
        {
            this.db = db;
            GenerateTestData();
        }
        private void GenerateTestData()
        {
            var query = from d in db.Users
                        where d.username.Equals("admin")
                        orderby d.firstname
                        select d;
            if (query.Count() < 1)
            {
                User user = new User();
                user.username = "admin";
                user.hashPassword = "e6b9e1f2f305a014b507954a8549bbbcf9f782c625b9f2d4fb7884e598189d87";
                user.lastname = "Adminson";
                user.firstname = "Admin";
                db.Users.Add(user);
                Commit();
            }
        }
        private int Commit()
        {
            return db.SaveChanges();
        }
        private bool CheckPermission(string tokken, string permission)
        {
            return true;
        }

        public bool CreateMember(string tokken, Member member)
        {
            throw new NotImplementedException();
        }

        public bool CreateUser(string tokken, User user)
        {
            db.Users.Add(user);
            Commit();
            return true;
            //throw new NotImplementedException();
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
            var query = from d in db.Users
                        where d.username.Equals(username) && d.hashPassword.Equals(password)
                        select d;
            if (query.Count() == 1)
            {
                Guid g = Guid.NewGuid();
                sessions.Add(new Session(query.First().username, query.First().Id, g.ToString(), new DateTime(), new Permissions()));
                return g.ToString();
            }
            else if (query.Count() > 1)
            {
                return "Error";
            }
            else
            {
                return "NULL";
            }
        }

        public bool Logout(string tokken, string username)
        {
            if (sessions.Exists(o => o.username.Equals(username) && o.tokken.Equals(tokken)))
            {
                sessions.Remove(sessions.Find(o => o.tokken.Equals(tokken)));
                return !ValidateTokken(tokken);
            }
            else
            {
                return false;
            }
        }

        public bool ValidateTokken(string tokken)
        {
            if (sessions.Exists(o => o.tokken.Equals(tokken))){
                return true;
            }
            else
            {
                return false;
            }
        }

        List<FinanceEntry> IServerSideData.GetFinans(string tokken, string konti, string searchvalue, string searchtype)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers(string tokken, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public List<Member> GetMembers(string tokken, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }
    }
}
