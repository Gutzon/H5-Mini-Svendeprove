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
        private bool CheckPermission(Validation validate, string permission)
        {
            return true;
        }

 

        public bool CreateUser(Validation validate, User user)
        {
            db.Users.Add(user);
            Commit();
            return true;
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

        public bool Logout(Validation validate)
        {
            if (sessions.Exists(o => o.username.Equals(validate.username) && o.tokken.Equals(validate.tokken)))
            {
                sessions.Remove(sessions.Find(o => o.tokken.Equals(validate.tokken)));
                return !ValidateTokken(validate);
            }
            else
            {
                return false;
            }
        }

        public bool ValidateTokken(Validation validate)
        {
            if (sessions.Exists(o => o.tokken.Equals(validate.tokken))){
                return true;
            }
            else
            {
                return false;
            }
        }

        public Permissions GetPermissions(Validation validate)
        {
            throw new NotImplementedException();
        }

        public bool EditUser(Validation validate, User user, User newuser)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(Validation validate, User user)
        {
            throw new NotImplementedException();
        }

        public bool CreateMember(Validation validate, Member member)
        {
            throw new NotImplementedException();
        }

        public bool EditMember(Validation validate, Member member, Member newmember)
        {
            throw new NotImplementedException();
        }

        public bool DeleteMember(Validation validate, Member member)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers(Validation validate, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public List<Member> GetMembers(Validation validate, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public List<FinanceEntry> GetFinances(Validation validate, string konti, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }
    }
}
