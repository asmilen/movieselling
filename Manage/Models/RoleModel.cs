using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class Role
    {
        public Role(int RoleID, string RoleName)
        {
            // TODO: Complete member initialization
            this.RoleID = RoleID;
            this.RoleName = RoleName;
        }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class ListRole
    {
        public List<Role> dropdownlistRole { get; set; }

        public ListRole()
        {
        }
    }
}