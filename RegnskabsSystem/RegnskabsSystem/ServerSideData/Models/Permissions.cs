﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class Permissions
    {
        

        public Permissions(bool addCorporation, bool admin, bool addUser, bool editUser, bool deleteUser, bool addMember, bool editMember, bool deleteMember, bool addFinance, bool viewFinance, bool limitedViewFinance, bool addInventory, bool editInventory, bool deleteInventory)
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
