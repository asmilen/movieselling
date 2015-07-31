using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage
{
    public class DatabaseHelper
    {
        //String Format
        public static String DateFormat = "dd-MM-yyyy";

        //Trang that dat ve
        public static String booked = "Đã đặt";
        public static String paid = "Đã thanh toán";

        // Bang Order
        public static String OrderID = "OrderID";
        public static String Email = "Email";
        public static String Cmnd = "Cmnd";
        public static String Phone = "Phone";
        public static String TotalPrice = "TotalPrice";

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

        //Room
        public static String NumberOfRow = "NumberOfRow";
        public static String NumberOfColumn = "NumberOfColumn";

        public static string TimeID = "TimeID";
        public static string Time = "Time";

        //List for View
        public static List<SelectListItem> listRoles = getlistRole(0);
        public static List<SelectListItem> listFilm = new List<SelectListItem>();
        public static List<SelectListItem> listTimes = getlistTime();
        public static List<SelectListItem> listRoom = getlistRoom();

        private static List<SelectListItem> getlistRoom()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            // Lay list role ra tu database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from Room";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int TimeIDr = (int)reader[DatabaseHelper.RoomID];
                        string Timer = reader[DatabaseHelper.Name].ToString();
                        listItems.Add(new SelectListItem() { Value = TimeIDr.ToString(), Text = Timer });
                    }
                }
            }

            return listItems;
        }

        private static List<SelectListItem> getlistTime()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            // Lay list role ra tu database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from TimeSchedule";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int TimeIDr = (int)reader[DatabaseHelper.TimeID];
                        string Timer = reader[DatabaseHelper.Time].ToString();
                        listItems.Add(new SelectListItem() { Value = TimeIDr.ToString(), Text = Timer });
                    }
                }
            }

            return listItems;
        }

        private static List<SelectListItem> getlistRole(int selected)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            // Lay list role ra tu database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from webpages_Roles";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int RoleIDr = (int)reader[DatabaseHelper.RoleID];
                        string RoleNamer = reader[DatabaseHelper.RoleName].ToString();
                        listItems.Add(new SelectListItem() { Value = RoleIDr.ToString(), Text = RoleNamer });
                    }
                }
            }

            // Gan gia tri selected default
            listItems.ElementAt(selected).Selected = true;

            return listItems;
        }

        public static void setNewSelectedRole(string roleName)
        {
            foreach (var item in listRoles)
            {
                item.Selected = false;
                if (item.Text.Equals(roleName))
                    item.Selected = true;
            }
        }

        public static string getRoomNamebyID(Int32 RoomID)
        {
            string Name = "";
            // Select ten film theo film ID
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            string sqlSelect = @"select Name from Room where RoomID = " + RoomID;
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sqlSelect, conn);
            conn.Open();
            System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Name = reader["Name"].ToString();
            }
            conn.Close();
            conn.Dispose();
            return Name;
        }

        public static string getFilmNamebyID(string FilmID)
        {
            string Name = "";
            // Select ten film theo film ID
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            string sqlSelect = @"select Name from Film where FilmID = " + FilmID;
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sqlSelect, conn);
            conn.Open();
            System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Name = reader["Name"].ToString();
            }
            conn.Close();
            conn.Dispose();
            return Name;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }


}