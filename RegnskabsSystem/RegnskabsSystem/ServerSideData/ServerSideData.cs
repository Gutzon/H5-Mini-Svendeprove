using System;
using System.Collections.Generic;
using ServerSideData.Models;
using System.Linq;
using ServerSideData.TransferModel;

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
                Corporation corporation1 = new()
                {
                    name = "Lillefnug",
                    cvrNummer = "11 11 11 11"
                };
                db.Corporations.Add(corporation1);
                Corporation corporation2 = new()
                {
                    name = "FDF",
                    cvrNummer = "22 22 22 22"
                };
                db.Corporations.Add(corporation2);
                Permissions perm1 = new(true, true, true, true, true, true, true, true, true, true, true, true, true, true);
                db.Permissions.Add(perm1);
                Permissions perm2 = new(true, true, true, true, true, true, true, true, true, true, true, true, true, true);
                db.Permissions.Add(perm2);
                Commit();
                User_Corp_Permission ucp = new()
                {
                    UserID = user.Id,
                    CorporationID = corporation1.ID,
                    PermissionID = perm1.ID
                };
                db.UCP.Add(ucp);
                ucp = new()
                {
                    UserID = user.Id,
                    CorporationID = corporation2.ID,
                    PermissionID = perm2.ID
                };
                db.UCP.Add(ucp);
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
                if (CheckPermission(validate, "lillefnug", "AddUser"))
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



        public UserLogin Login(string username, string password)
        {
            var query = from d in db.Users
                        where d.username.Equals(username) && d.hashPassword.Equals(password)
                        select d;
            UserLogin userLogin = new();
            if (query.Count() == 1)
            {
                var corpQuery = from user in db.Users
                                join ucp in db.UCP on user.Id equals ucp.UserID
                                join corp in db.Corporations on ucp.CorporationID equals corp.ID
                                where user.username.Equals(query.First().username)
                                orderby corp.name
                                select corp;

                userLogin = new()
                {
                    tokken = Guid.NewGuid().ToString(),
                    Corporations = corpQuery.ToList<Corporation>()
                };
                sessions.Add(new Session(query.First().username, query.First().Id, userLogin.tokken, DateTime.Now));
                if (corpQuery.Count() == 1)
                {
                    if (SelectCorporation(new Validation(username, userLogin.tokken), corpQuery.First().ID))
                    {
                        userLogin.status = "OK";
                        
                    }
                    else
                    {
                        userLogin.status = "Fail";
                    }
                }
                else if (SelectCorporation(new Validation(username, userLogin.tokken), corpQuery.First().ID))
                {
                    userLogin.status = "Select";
                }
                return userLogin;
            }
            else
            {
                userLogin.status = "Error";
                return userLogin;
            }
        }
        public bool SelectCorporation(Validation validate, int ID)
        {
            if (ValidateTokken(validate))
            {
                var corpQuery = from user in db.Users
                                join ucp in db.UCP on user.Id equals ucp.UserID
                                where user.username.Equals(validate.username) && ucp.CorporationID.Equals(ID)
                                select ucp;
                if (corpQuery.Count() == 1)
                {
                    var Query = from perm in db.Permissions
                                join ucp in db.UCP on perm.ID equals ucp.PermissionID
                                join user in db.Users on ucp.UserID equals user.Id
                                where user.username.Equals(validate.username) && ucp.CorporationID.Equals(ID)
                                select perm;
                    sessions.Find(o => o.tokken.Equals(validate.tokken) && o.username.Equals(validate.username)).permissions = Query.First();
                    sessions.Find(o => o.tokken.Equals(validate.tokken) && o.username.Equals(validate.username)).corporationId = ID;
                    return true;
                }
            }
            return false;
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
                sessions.Find(o => o.tokken.Equals(validate.tokken) && o.username.Equals(validate.username)).lastUsed = DateTime.Now;
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

        public IEnumerable<User> GetUsers(Validation validate, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Member> GetMembers(Validation validate, string searchvalue = "", string searchtype = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FinanceEntry> GetFinances(Validation validate, string konti = "", string searchvalue = "", string searchtype = "")
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
