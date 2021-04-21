using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSideData.TransferModel;

namespace ServerSideData.Models
{
    public class Permissions
    {
        public Permissions(TransferPermissions permissions)
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
        public Permissions(Permissions perm, TransferPermissions permissions)
        {
            ID = perm.ID;
            if (perm.AddCorporation)
            {
                AddCorporation = true;
            }
            else
            {
                AddCorporation = permissions.AddCorporation;
            }
            if (perm.Admin)
            {
                Admin = true;
            }
            else
            {
                Admin = permissions.Admin;
            }
            if (perm.AddUser)
            {
                AddUser = true;
            }
            else
            {
                AddUser = permissions.AddUser;
            }
            if (perm.EditUser)
            {
                EditUser = true;
            }
            else
            {
                EditUser = permissions.EditUser;
            }
            if (perm.DeleteUser)
            {
                DeleteUser = true;
            }
            else
            {
                DeleteUser = permissions.DeleteUser;
            }
            if (perm.AddMember)
            {
                AddMember = true;
            }
            else
            {
                AddMember = permissions.AddMember;
            }
            if (perm.EditMember)
            {
                EditMember = true;
            }
            else
            {
                EditMember = permissions.EditMember;
            }
            if (perm.DeleteMember)
            {
                DeleteMember = true;
            }
            else
            {
                DeleteMember = permissions.DeleteMember;
            }
            if (perm.AddFinance)
            {
                AddFinance = true;
            }
            else
            {
                AddFinance = permissions.AddFinance;
            }
            if (perm.ViewFinance)
            {
                ViewFinance = true;
            }
            else
            {
                ViewFinance = permissions.ViewFinance;
            }
            if (perm.LimitedViewFinance)
            {
                LimitedViewFinance = true;
            }
            else
            {
                LimitedViewFinance = permissions.LimitedViewFinance;
            }
            if (perm.AddInventory)
            {
                AddInventory = true;
            }
            else
            {
                AddInventory = permissions.AddInventory;
            }
            if (perm.EditInventory)
            {
                EditInventory = true;
            }
            else
            {
                EditInventory = permissions.EditInventory;
            }
            if (perm.DeleteInventory)
            {
                DeleteInventory = true;
            }
            else
            {
                DeleteInventory = permissions.DeleteInventory;
            }
        }
        public void Update(TransferPermissions permissions)
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

        public Permissions(bool addCorporation = false, bool admin = false, bool addUser = false, bool editUser = false, bool deleteUser = false, bool addMember = false, bool editMember = false, bool deleteMember = false, bool addFinance = false, bool viewFinance = false, bool limitedViewFinance = false, bool addInventory = false, bool editInventory = false, bool deleteInventory = false)
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

        public int ID { get; set; }
        public int UserID { get; set; }
        public int CorporationID { get; set; }
        public bool AddCorporation { get; set; }
        public bool Admin { get; set; }
        public bool AddUser { get; set; }
        public bool EditUser { get; set; }
        public bool DeleteUser { get; set; }
        public bool AddMember { get; set; }
        public bool EditMember { get; set; }
        public bool DeleteMember { get; set; }
        public bool AddFinance { get; set; }
        public bool ViewFinance { get; set; }
        public bool LimitedViewFinance { get; set; }
        public bool AddInventory { get; set; }
        public bool EditInventory { get; set; }
        public bool DeleteInventory { get; set; }



    }
}
