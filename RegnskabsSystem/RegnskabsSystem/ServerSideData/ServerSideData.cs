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
                db.Users.RemoveRange(db.Users.Where(o => o.Id >= 0));
                db.Corporations.RemoveRange(db.Corporations.Where(o => o.ID >= 0));
                db.FinanceEntries.RemoveRange(db.FinanceEntries.Where(o => o.ID >= 0));
                db.Inventories.RemoveRange(db.Inventories.Where(o => o.ID >= 0));
                db.Kontis.RemoveRange(db.Kontis.Where(o => o.ID >= 0));
                db.Members.RemoveRange(db.Members.Where(o => o.ID >= 0));
                db.Permissions.RemoveRange(db.Permissions.Where(o => o.ID >= 0));
                db.UCP.RemoveRange(db.UCP.Where(o => o.ID >= 0));
                User user = new()
                {
                    username = "admin",
                    hashPassword = "e6b9e1f2f305a014b507954a8549bbbcf9f782c625b9f2d4fb7884e598189d87",
                    lastname = "Adminson",
                    firstname = "Admin",
                    mail = "admin@adminson.local"
                };
                db.Users.Add(user);
                Corporation corporation = new()
                {
                    name = "Lillefnug",
                    cvrNummer = "11 11 11 11"
                };
                db.Corporations.Add(corporation);
                Commit();
            }
        }
        private int Commit()
        {
            return db.SaveChanges();
        }
        private bool CheckPermission(Validation validate, string corporation , string permission)
        {
            if (ValidateTokken(validate))
            {

            var query = from user in db.Users
                        join link in db.UCP on user.Id equals link.UserID
                        join perm in db.Permissions on link.PermissionID equals perm.ID
                        where user.username.Equals(validate.username) 
                        orderby user.firstname
                        select perm;
            return true;
            }
            return false;
        }



        public bool CreateUser(Validation validate, User user)
        {
            if (ValidateTokken(validate))
            {
                if (CheckPermission(validate, "lillfnug", "AddUser"))
                {
                    db.Users.Add(user);
                    Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        public string Login(string username, string password, int corporation)
        {
            var query = from d in db.Users
                        where d.username.Equals(username) && d.hashPassword.Equals(password)
                        select d;
            if (query.Count() == 1)
            {
                Guid g = Guid.NewGuid();
                sessions.Add(new Session(query.First().username, query.First().Id, g.ToString(), DateTime.Now, new Permissions(), corporation));
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
            if (ValidateTokken(validate))
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
            if (sessions.Exists(o => o.tokken.Equals(validate.tokken) && o.username.Equals(validate.username)))
            {
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

        public IEnumerable<User> GetUsers(Validation validate, string corporation = "", string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Member> GetMembers(Validation validate, string corporation = "", string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FinanceEntry> GetFinances(Validation validate, string konti = "", string corporation = "", string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Corporation> GetCorporations()
        {
            var query = from d in db.Corporations
                        orderby d.name
                        select d;
            return query;
        }
    }
}
