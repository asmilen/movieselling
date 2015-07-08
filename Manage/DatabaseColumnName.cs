using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manage
{
    public class DatabaseColumnName
    {
        //String Format
        public static String DateFormat = "dd-MM-yyyy";

        //Film
        public static String FilmID = "FilmID";
        public static String UserID = "UserID";
        public static String CategoryID = "CategoryID";
        public static String Name = "Name";
        public static String Actor = "Actor";
        public static String Director = "Director";
        public static String Description = "Description";
        public static String Picture = "Picture";
        public static String StartDate = "StartDate";
        public static String EndDate = "EndDate";
        public static String Password = "password";

        //Role
        public static String RoleID = "RoleID";
        public static String RoleName = "RoleName";

        //User
        public static String FullName = "Name";
        public static String DateOfBirth = "DateOfBirth";
        public static String username = "username";
        public static String Address = "Address";

        //Tech
        public static String TechID = "TechID";
        public static String Feature = "Feature";

        //Schedule
        public static String ScheduleID = "ScheduleID";
        public static String RoomID = "RoomID";
        public static String StartTime = "StartTime";
        public static String DateSche = "DateSche";
    }
}