using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.Models;

namespace ServerSideData.TransferModel
{
    public class TransferPermissions
    {
        public TransferPermissions(bool addCorporation, bool admin, bool addUser, bool editUser, bool deleteUser, bool addMember, bool editMember, bool deleteMember, bool addFinance, bool viewFinance, bool limitedViewFinance, bool addInventory, bool editInventory, bool deleteInventory)
        {
            AddCorporation = addCorporation;
            Admin = admin;
            AddUser = addUser;
            EditUser = editUser;
            DeleteUser = deleteUser;
            AddMember = addMember;
            EditMember = editMember;
            DeleteMember = deleteMember;
            AddFinance = addFinance;
            ViewFinance = viewFinance;
            LimitedViewFinance = limitedViewFinance;
            AddInventory = addInventory;
            EditInventory = editInventory;
            DeleteInventory = deleteInventory;
        }
        public TransferPermissions(Permissions permissions)
        {
            AddCorporation = permissions.AddCorporation;
            Admin = permissions.Admin;
            AddUser = permissions.AddUser;
            EditUser = permissions.EditUser;
            DeleteUser = permissions.DeleteUser;
            AddMember = permissions.AddMember;
            EditMember = permissions.EditMember;
            DeleteMember = permissions.DeleteMember;
            AddFinance = permissions.AddFinance;
            ViewFinance = permissions.ViewFinance;
            LimitedViewFinance = permissions.LimitedViewFinance;
            AddInventory = permissions.AddInventory;
            EditInventory = permissions.EditInventory;
            DeleteInventory = permissions.DeleteInventory;
        }

        public bool AddCorporation { get; }
        public bool Admin { get; }
        public bool AddUser { get; }
        public bool EditUser { get; }
        public bool DeleteUser { get; }
        public bool AddMember { get; }
        public bool EditMember { get; }
        public bool DeleteMember { get; }
        public bool AddFinance { get; }
        public bool ViewFinance { get; }
        public bool LimitedViewFinance { get; }
        public bool AddInventory { get; }
        public bool EditInventory { get; }
        public bool DeleteInventory { get; }
    }
}
