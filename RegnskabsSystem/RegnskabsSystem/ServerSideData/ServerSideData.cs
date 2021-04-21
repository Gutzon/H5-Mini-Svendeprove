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
        private static List<RepFinanceEntry> repFinList = new();

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
                Konti konti1 = new()
                {
                    CorporationID = corporation1.ID,
                    name = "Main"
                };
                db.Kontis.Add(konti1);
                Konti konti2 = new()
                {
                    CorporationID = corporation2.ID,
                    name = "Main"
                };
                db.Kontis.Add(konti2);
                Commit();
                FinanceEntry entry1 = new()
                {
                    KontiID = konti1.ID,
                    value = 10000,
                    comment = "Start Value",
                    byWho = "admin",
                    newSaldoKonti = 10000,
                    newSaldoMain = 10000,
                    addDate = DateTime.Now,
                    payDate = DateTime.Now
                };
                db.FinanceEntries.Add(entry1);
                FinanceEntry entry2 = new()
                {
                    KontiID = konti2.ID,
                    value = 9000,
                    comment = "Start Value",
                    byWho = "admin",
                    newSaldoKonti = 9000,
                    newSaldoMain = 9000,
                    addDate = DateTime.Now,
                    payDate = DateTime.Now
                };
                db.FinanceEntries.Add(entry2);
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
        private void GenerateRepTestData()
        {
            db.RepFinanceEntries.Add(new RepFinanceEntry()
            {
                KontiID = 5,
                value = 20,
                comment = "Test",
                byWho = "Who Knows",
                intervalType = "Hour",
                intervalValue = 1,
                firstExecDate = new DateTime(2021, 4, 20, 5, 0, 0),
                nextExecDate = new DateTime(2021, 4, 20, 5, 0, 0),
            });
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
                UpdateRepFinList();
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
            }
            else
            {
                userLogin.status = "Error";
            }
                return userLogin;
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
                db.Users.Where(o => o.username.Equals(validate.username)).First().lastSeen = DateTime.Now;
                Commit();
                return true;
            }
            else
            {
                return false;
            }
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
                        query.First().users.firstname = resultuser.firstname;
                        query.First().users.lastname = resultuser.lastname;
                        query.First().users.mail = resultuser.mail;
                        query.First().users.firstname = resultuser.firstname;
                        Permissions newPerm = query.First().perm;
                        newPerm.Update(query.First().perm, newuser.permissions);
                        db.Users.Update(query.First().users);
                        db.Permissions.Update(newPerm);
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
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                if (CheckPermission(validate, ses, "AddMember"))
                {
                    if (member.firstname != "" && member.lastname != "")
                    {
                        member.CorporationID = ses.corporationId;
                        db.Members.Add(member);
                        Commit();
                        return "OK";
                    }
                    return "Name empty";
                }
                return "Not permited";
            }
            return "No session";
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
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
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
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
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
        public string AddFinance(Validation validate, TransferFinance financeIn)
        {
            if (ValidateTokken(validate))
            {
                UpdateRepFinList();
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance"))
                {
                    var query = from kontis in db.Kontis
                                where kontis.CorporationID.Equals(ses.corporationId) && kontis.name.Equals(financeIn.konti)
                                select kontis;
                    if (query.Count() == 1)
                    {
                        var query2 = from entries in db.FinanceEntries
                                     join kontis in db.Kontis on entries.KontiID equals kontis.ID
                                     where kontis.CorporationID.Equals(ses.corporationId)
                                     orderby entries.payDate
                                     select new { entries, kontis };
                        FinanceEntry newEntry = new()
                        {
                            value = financeIn.value,
                            KontiID = query.First().ID,
                            comment = financeIn.comment,
                            byWho = ses.username,
                            addDate = DateTime.Now,
                            payDate = DateTime.Now
                        };
                        if (query2.Where(o => o.kontis.name.Equals(financeIn.konti)).Any())
                        {
                            newEntry.newSaldoKonti = query2.Where(o => o.kontis.name.Equals(financeIn.konti)).Last().entries.newSaldoKonti + financeIn.value;
                        }
                        else
                        {
                            newEntry.newSaldoKonti = financeIn.value;
                        }
                        if (query2.Any())
                        {
                            newEntry.newSaldoMain = query2.Last().entries.newSaldoMain + financeIn.value;
                        }
                        else
                        {
                            newEntry.newSaldoKonti = financeIn.value;
                        }

                        db.FinanceEntries.Add(newEntry);
                        Commit();
                        return "ok";

                    }
                    return "Wrong Konti name";
                }
                return "not permitted";
            }
            return "no session";
        }
        public string AddKonti(Validation validate, string name)
        {
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                name = name.Trim();
                if ((CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance")) && name != "")
                {
                    var query = from kontis in db.Kontis
                                where kontis.CorporationID.Equals(ses.corporationId) && kontis.name.Equals(name)
                                orderby kontis.name
                                select kontis;
                    if (!query.Any())
                    {
                        db.Kontis.Add(new Konti() {name = name, CorporationID = ses.corporationId });
                        Commit();
                        return "OK";
                    }
                    return "Alredy exist";
                }
                return "Not permitted";
            }
            return "No session";
        }
        public string ChangeKontiName(Validation validate, string oldname, string newname)
        {
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                oldname = oldname.Trim();
                newname = newname.Trim();
                if ((CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance")) && oldname != "" && newname != "" && oldname != "Main" && newname != "Main")
                {
                    var query = from kontis in db.Kontis
                                where kontis.CorporationID.Equals(ses.corporationId) && kontis.name.Equals(oldname)
                                orderby kontis.name
                                select kontis;
                    if (query.Count() == 1 && !db.Kontis.Where(o => o.CorporationID.Equals(ses.corporationId) && o.name.Equals(newname)).Any())
                    {
                        query.First().name = newname;
                        db.Kontis.Update(query.First());
                        Commit();
                        return "OK";
                    }
                    return "Name problem";
                }
                return "Not permitted";
            }
            return "No session";
        }
        public IEnumerable<TransferFinance> GetFinances(Validation validate, string konti = "", string searchvalue = "", string searchtype = "")
        {
            List<TransferFinance> finacelist = new();
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                konti = konti.Trim();
                UpdateRepFinList();
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance") || CheckPermission(validate, ses, "ViewFinace") || CheckPermission(validate, ses, "LimitedViewFinance"))
                {
                    var query = from finaces in db.FinanceEntries
                                join kontis in db.Kontis on finaces.KontiID equals kontis.ID
                                where finaces.ID.Equals(0)
                                select new {finaces, kontis};
                    bool kontifiltered = true;
                    if (konti == "" || konti == "Main")
                    {
                        kontifiltered = false;
                        query = from finaces in db.FinanceEntries
                                join kontis in db.Kontis on finaces.KontiID equals kontis.ID
                                where kontis.CorporationID.Equals(ses.corporationId)
                                orderby finaces.ID
                                select new {finaces, kontis};
                    }
                    else
                    {
                        query = from finaces in db.FinanceEntries
                                join kontis in db.Kontis on finaces.KontiID equals kontis.ID
                                where kontis.CorporationID.Equals(ses.corporationId) && kontis.name.Equals(konti)
                                orderby finaces.ID
                                select new {finaces, kontis};
                    }

                    switch (searchtype)
                    {
                        case "Test":


                            break;
                        default:
                            break;
                    }
                    if (ses.permissions.AddFinance || ses.permissions.ViewFinance)
                    {
                        foreach (var q in query)
                        {
                            finacelist.Add(new TransferFinance(q.finaces, q.kontis, "Full", kontifiltered));
                        }
                    }
                    else
                    {
                        foreach (var q in query)
                        {
                            finacelist.Add(new TransferFinance(q.finaces, q.kontis, "Limited", kontifiltered));
                        }
                    }
                    
                }

            }
            return finacelist;
        }
        public IEnumerable<Corporation> GetCorporations()
        {
            var query = from d in db.Corporations
                        orderby d.name
                        select d;
            return query;
        }
        public IEnumerable<string> GetKonties(Validation validate)
        {
            List<string> kontiList = new();
            if (ValidateTokken(validate))
            {
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance") || CheckPermission(validate, ses, "ViewFinace") || CheckPermission(validate, ses, "LimitedViewFinance"))
                {
                    var query = from kontis in db.Kontis 
                                where kontis.CorporationID.Equals(ses.corporationId)
                                orderby kontis.name
                                select kontis;
                    
                        foreach (var q in query)
                        {
                        kontiList.Add(q.name);
                        }
                    
                }

            }
            return kontiList;
        }

        public string AddRepFinance(Validation validate, TransferRepFinance transferRepFinance)
        {
            if (ValidateTokken(validate))
            {
                UpdateRepFinList();
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance"))
                {
                   /* var query = from kontis in db.Kontis
                                where kontis.CorporationID.Equals(ses.corporationId) && kontis.name.Equals(transferRepFinance.konti)
                                select kontis;
                    if (query.Count() == 1)
                    {
                        var query2 = from entries in db.FinanceEntries
                                     join kontis in db.Kontis on entries.KontiID equals kontis.ID
                                     where kontis.CorporationID.Equals(ses.corporationId)
                                     orderby entries.payDate
                                     select new { entries, kontis };

                        db.FinanceEntries.Add(newEntry);
                        Commit();
                        return "ok";

                    }
                    return "Wrong Konti name";*/
                }
                return "not permitted";
            }
            return "no session";
        }

        public string RemoveRepFinance(Validation validate, TransferRepFinance transferRepFinance)
        {
            UpdateRepFinList();
            throw new NotImplementedException();
        }

        public IEnumerable<TransferRepFinance> GetRepFinance(Validation validate, string konti = "")
        {
            List<TransferRepFinance> repFinacelist = new();
            if (ValidateTokken(validate))
            {
                UpdateRepFinList();
                Session ses = sessions.Find(o => o.tokken.Equals(validate.tokken));
                konti = konti.Trim();
                if (CheckPermission(validate, ses, "AddCorporation") || CheckPermission(validate, ses, "Admin") || CheckPermission(validate, ses, "AddFinance") || CheckPermission(validate, ses, "ViewFinace") || CheckPermission(validate, ses, "LimitedViewFinance"))
                {
                    var query = from repFinaces in db.RepFinanceEntries
                                join kontis in db.Kontis on repFinaces.KontiID equals kontis.ID
                                where repFinaces.ID.Equals(0)
                                select new { repFinaces, kontis };
                    if (konti == "" || konti == "Main")
                    {
                        query = from repFinaces in db.RepFinanceEntries
                                join kontis in db.Kontis on repFinaces.KontiID equals kontis.ID
                                where kontis.CorporationID.Equals(ses.corporationId)
                                orderby repFinaces.ID
                                select new { repFinaces, kontis };
                    }
                    else
                    {
                        query = from repFinaces in db.RepFinanceEntries
                                join kontis in db.Kontis on repFinaces.KontiID equals kontis.ID
                                where kontis.CorporationID.Equals(ses.corporationId) && kontis.name.Equals(konti)
                                orderby repFinaces.ID
                                select new { repFinaces, kontis };
                    }

                    if (ses.permissions.AddFinance || ses.permissions.ViewFinance)
                    {
                        foreach (var q in query)
                        {
                            repFinacelist.Add(new TransferRepFinance(q.repFinaces, q.kontis, "Full"));
                        }
                    }
                    else
                    {
                        foreach (var q in query)
                        {
                            repFinacelist.Add(new TransferRepFinance(q.repFinaces, q.kontis, "Limited"));
                        }
                    }

                }

            }
            return repFinacelist;
        }
        private void UpdateRepFinList()
        {
            var query = from repFin in db.RepFinanceEntries
                        select repFin;
            repFinList = query.ToList();
            while (repFinList.Where(o => o.nextExecDate <= DateTime.Now).Any()){
                foreach (RepFinanceEntry entry in repFinList.Where(o => o.nextExecDate <= DateTime.Now))
                {
                    FinanceEntry newEntry = new()
                    {
                        value = entry.value,
                        KontiID = entry.KontiID,
                        comment = "\"Recurring\": " + entry.comment,
                        byWho = entry.byWho,
                        addDate = entry.nextExecDate,
                        payDate = entry.nextExecDate,
                    };
                    var query2 = from entries in db.FinanceEntries
                                 join kontis in db.Kontis on entries.KontiID equals kontis.ID
                                 where kontis.CorporationID.Equals(db.Kontis.Find(entry.KontiID).CorporationID)
                                 orderby entries.payDate
                                 select entries;

                    if (query2.Where(o => o.KontiID.Equals(entry.KontiID)).Any())
                    {
                        newEntry.newSaldoKonti = query2.Where(o => o.KontiID.Equals(entry.KontiID)).Last().newSaldoKonti + entry.value;
                    }
                    else
                    {
                        newEntry.newSaldoKonti = entry.value;
                    }
                    if (query2.Any())
                    {
                        newEntry.newSaldoMain = query2.Last().newSaldoMain + entry.value;
                    }
                    else
                    {
                        newEntry.newSaldoKonti = entry.value;
                    }
                    switch (entry.intervalType)
                    {
                        case "Year":
                            entry.nextExecDate = entry.nextExecDate.AddYears(entry.intervalValue);
                            break;
                        case "Month":
                            if (entry.firstExecDate.Day < 28)
                            {
                                entry.nextExecDate = entry.nextExecDate.AddMonths(entry.intervalValue);
                            }
                            else
                            {
                                DateTime newDate = new DateTime(entry.nextExecDate.Year, entry.nextExecDate.Month + entry.intervalValue, 27);
                                while (newDate.AddDays(1).Month.Equals(newDate.Month) && newDate.Day < entry.firstExecDate.Day)
                                {
                                    newDate = newDate.AddDays(1);
                                }
                                entry.nextExecDate = newDate;
                            }
                            break;
                        case "Week":
                            entry.nextExecDate = entry.nextExecDate.AddDays(entry.intervalValue * 7);
                            break;
                        case "Day":
                            entry.nextExecDate = entry.nextExecDate.AddDays(entry.intervalValue);
                            break;
                        case "Hour":
                            entry.nextExecDate = entry.nextExecDate.AddHours(entry.intervalValue);
                            break;
                    }
                    db.RepFinanceEntries.Update(entry);
                    db.FinanceEntries.Add(newEntry);
                    Commit();
                }
            }

        }
    }
}
