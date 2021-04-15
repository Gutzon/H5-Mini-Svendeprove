using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData.Models
{
    public class User_Corp_Permission
    {
        public User_Corp_Permission()
        {
        }
        public User_Corp_Permission(int userID, int permissionID, int corporationID)
        {
            UserID = userID;
            PermissionID = permissionID;
            CorporationID = corporationID;
        }

        public int ID { get; set; }
        public int UserID { get; set; }
        public int PermissionID { get; set; }
        public int CorporationID { get; set; }
    }
}
