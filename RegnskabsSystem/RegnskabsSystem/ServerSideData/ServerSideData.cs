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
                User user1 = new()
                {
                    username = "admin",
                    hashPassword = "e6b9e1f2f305a014b507954a8549bbbcf9f782c625b9f2d4fb7884e598189d87",
                    lastname = "Adminson",
                    firstname = "Admin",
                    mail = "admin@adminson.local"
                };
                db.Users.Add(user1);
                User user2 = new()
                {
                    username = "Lillefnug",
                    hashPassword = "ca5cebd439d176c601d6b53183f715022211ce33b2cb9267635129b6f2736882",
                    lastname = "Lille",
                    firstname = "Fnug",
                    mail = "admin@lillefnug.local"
                };
                db.Users.Add(user2);
                User user3 = new()
                {
                    username = "FDF",
                    hashPassword = "cf71e45b83a043690594715cdba1ee7cbc291f825ad09aa5456b686d0a969bc7",
                    lastname = "FDF",
                    firstname = "FDF",
                    mail = "admin@FDF.local"
                };
                db.Users.Add(user3);
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
                Permissions perm3 = new(false, true, true, true, true, true, true, true, true, true, true, true, true, true);
                db.Permissions.Add(perm3);
                Permissions perm4 = new(false, true, true, true, true, true, true, true, true, true, true, true, true, true);
                db.Permissions.Add(perm4);
                Commit();
                User_Corp_Permission ucp = new()
                {
                    UserID = user1.Id,
                    CorporationID = corporation1.ID,
                    PermissionID = perm1.ID
                };
                db.UCP.Add(ucp);
                ucp = new()
                {
                    UserID = user1.Id,
                    CorporationID = corporation2.ID,
                    PermissionID = perm2.ID
                };
                db.UCP.Add(ucp);
                ucp = new()
                {
                    UserID = user2.Id,
                    CorporationID = corporation1.ID,
                    PermissionID = perm3.ID
                };
                db.UCP.Add(ucp);
                ucp = new()
                {
                    UserID = user3.Id,
                    CorporationID = corporation2.ID,
                    PermissionID = perm4.ID
                };
                db.UCP.Add(ucp);
                Commit();
                AddMembers();
            }
        }

        private void AddMembers()
        {
            var lilleFnugId = GetCorporations().FirstOrDefault(c => c.name == "Lillefnug").ID;
            Member member1 = new Member()
            {
                CorporationID = lilleFnugId,
                firstname = "Trine",
                lastname = "Jensen",
                mail = "Trine.Jensen@some-place-beyond.com",
                phoneNumber = "66221133"
            };
            db.Members.Add(member1);
            Member member2 = new Member()
            {
                CorporationID = lilleFnugId,
                firstname = "Peter",
                lastname = "Lease",
                mail = "Peter.Leasy@i-drive-a-yellow-truck.com",
                phoneNumber = "88888888"
            };
            db.Members.Add(member2);
            Member member3 = new Member()
            {
                CorporationID = lilleFnugId,
                firstname = "Lars",
                lastname = "Larsen",
                mail = "Lars.Larsen@sssh-jeg-sover.dk",
                phoneNumber = "87123456"
            };
            db.Members.Add(member3);
            var fdfId = GetCorporations().FirstOrDefault(c => c.name == "FDF").ID;
            Member member4 = new Member()
            {
                CorporationID = fdfId,
                firstname = "Hans",
                lastname = "Poulsen",
                mail = "superspejderen@jeg-er-leet.dk",
                phoneNumber = "85012654"
            };
            db.Members.Add(member4);
            Member member5 = new Member()
            {
                CorporationID = fdfId,
                firstname = "Josefine-Petronella",
                lastname = "Birgensen-Birkefryd",
                mail = "Josefine.Petronella.Birgensen.Birkefryd@os-med-de-lange-navne.dk",
                phoneNumber = "80012654"
            };
            db.Members.Add(member5);
            Commit();
        }

        private int Commit()
        {
            return db.SaveChanges();
        }
        private bool CheckPermission(Validation validate, Session ses, string permission)
        {
            if (ValidateTokken(validate))
            {
                if ((bool)typeof(Permissions).GetProperty(permission).GetValue(ses.permissions, null))
                {
                    return true;
                }
            }
            return false;
        }

        public string CreateUser(Validation validate, TransferUser user)
        {
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                if (CheckPermission(validate, ses, "AddUser"))
                {
                    if (db.Users.Where(o => o.username.Equals(user.username)).Count() == 0 && user.username != "" && user.hashPassword != "")
                    {
                        var newusercheck = CheckUserPermissions(new TransferUser(new User(ses), ses.permissions), user);
                        if (newusercheck.Item1)
                        {
                            User newuser = new User(newusercheck.Item2);
                            Permissions perm = new(newusercheck.Item2.permissions);
                            db.Users.Add(newuser);
                            db.Permissions.Add(perm);
                            Commit();
                            db.UCP.Add(new User_Corp_Permission(newuser.Id, perm.ID, ses.corporationId));
                            Commit();
                            return "OK";
                        }
                        return "Failed setting permission check";
                    }
                    return "Username already in use";
                }
                return "Not permited";
            }
            return "No session";
        }
        private (bool, TransferUser) CheckUserPermissions(TransferUser olduser, TransferUser newuser)
        {
            if ((olduser.permissions.AddCorporation || !olduser.permissions.AddCorporation) && (newuser.permissions.AddCorporation || !newuser.permissions.AddCorporation))
            {

                Dictionary<string, bool> olduserperm = PermissionToDictionary(olduser.permissions);
                Dictionary<string, bool> newuserperm = PermissionToDictionary(newuser.permissions);
                if (olduserperm.Where(o => o.Key.Equals("AddCorporation")).First().Value)
                {

                }
                else
                {
                    foreach (var obj in newuserperm.Where(o => o.Value.Equals(true)))
                    {
                        if (!((olduserperm.Where(o => o.Key.Equals(obj.Key)).First().Value || olduserperm.Where(o => o.Key.Equals("Admin")).First().Value) && obj.Key != "AddCorporation"))
                        {
                            newuserperm[obj.Key] = false;
                            typeof(TransferPermissions).GetProperty(obj.Key).SetValue(newuser.permissions, false);
                        }
                    }
                }
                return (true, newuser);
            }
            return (false, olduser);
        }
        private Dictionary<string, bool> PermissionToDictionary(TransferPermissions perm)
        {
            Dictionary<string, bool> dict = new();
            foreach (var p in typeof(TransferPermissions).GetProperties().Select(o =>
            {
                object value = o.Name;
                return value == null ? null : value.ToString();
            }).ToArray())
            {
                dict.Add(p, (bool)typeof(TransferPermissions).GetProperty(p).GetValue(perm, null));
            }
            return dict;
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
                                select corp ;

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
                    var Query = from user in db.Users
                                join ucp in db.UCP on user.Id equals ucp.UserID
                                join perm in db.Permissions on ucp.PermissionID equals perm.ID
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

        public bool EditUser(Validation validate, TransferUser user, TransferUser newuser)
        {
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                if ((CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "EditUser") || user.username.Equals(ses.username)) && user.username.Equals(newuser.username))
                {
                    var query = from users in db.Users
                                join ucp in db.UCP on users.Id equals ucp.UserID
                                join perm in db.Permissions on ucp.PermissionID equals perm.ID
                                where users.username.Equals(user.username) && ucp.CorporationID.Equals(ses.corporationId)
                                select new {users, perm};
                    if ((query.Count() == 1 && !(query.First().perm.AddCorporation || query.First().perm.Admin)) || ses.username.Equals(query.First().users.username) || (ses.permissions.AddCorporation || ses.permissions.Admin))
                    {
                        TransferUser resultuser = new(query.First().users, query.First().perm);
                        newuser = CheckUserPermissions(resultuser, newuser).Item2;
                        db.Users.Update(new User(query.First().users, resultuser));
                        db.Permissions.Update(new Permissions(query.First().perm, resultuser.permissions));
                        Commit();
                        return true;
                    }

                }
            }
            return false;
        }

        public bool DeleteUser(Validation validate, TransferUser user)
        {
            throw new NotImplementedException();
        }

        public string CreateMember(Validation validate, Member member)
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

        public IEnumerable<TransferUser> GetUsers(Validation validate, string searchvalue = "", string searchtype = "")
        {
            List<TransferUser> userlist = new();
            Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
            if (ValidateTokken(validate))
            {
                var query = from users in db.Users
                            join ucp in db.UCP on users.Id equals ucp.UserID
                            where ucp.CorporationID.Equals(ses.corporationId) && users.username.Equals(ses.username)
                            orderby users.firstname
                            select users;
                userlist.Add(new TransferUser(query.ToArray()[0], ses.permissions));
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "EditUser") || CheckPermission(validate, ses, "DeleteUser") || CheckPermission(validate, ses, "AddUser"))
                {

                
                switch (searchtype)
                {
                        case "Test":

                        
                        break;
                        case "Self":

                            break;
                    default:
                        query = from users in db.Users
                                join ucp in db.UCP on users.Id equals ucp.UserID
                                where ucp.CorporationID.Equals(ses.corporationId) && !users.username.Equals(ses.username)
                                orderby users.firstname
                                select users;
                        break;
                }
                    IEnumerable<User> userresult = query.ToArray();
                    foreach (User user in userresult)
                    {
                        user.hashPassword = "";
                        var permquery = from perm in db.Permissions
                                        join ucp in db.UCP on perm.ID equals ucp.PermissionID
                                        where ucp.UserID.Equals(user.Id) && ucp.CorporationID.Equals(ses.corporationId)
                                        select perm;
                        userlist.Add(new TransferUser(user, permquery.First()));

                    }

                }
                
            }
            return userlist;
        }

        public IEnumerable<Member> GetMembers(Validation validate, string searchvalue = "", string searchtype = "")
        {
            List<Member> memberlist = new();
            Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
            if (ValidateTokken(validate))
            {
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "EditMember") || CheckPermission(validate, ses, "DeleteMember") || CheckPermission(validate, ses, "AddMember"))
                {
                    var query = from members in db.Members
                                where members.ID.Equals(0)
                                select members;

                    switch (searchtype)
                    {
                        case "Test":


                            break;
                        default:
                            query = from members in db.Members
                                    where members.CorporationID.Equals(ses.corporationId)
                                    orderby members.firstname
                                    select members;
                            break;
                    }
                    foreach (Member member in query)
                    {
                        memberlist.Add(member);

                    }
                }

            }
            return memberlist;
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
