using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieSelling
{
    public static class DatabaseHelper
    {
        //String Format
        public static String DateFormat = "dd-MM-yyyy";

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
        public static String Company = "Company";
        public static String filmLong = "FilmLong";
        public static String Picture1 = "Picture1";
        public static String Picture2 = "Picture2";
        public static String Picture3 = "Picture3";
        public static String Picture4 = "Picture4";

        // Bang Schedule
        public static String ScheduleID = "ScheduleID";
        public static String StartTime = "StartTime";
        public static string Price = "price";

        // Bang Category
        public static String TechID = "TechID";
        public static String Feature = "Feature";

        //Bang room
        public static String RoomID = "RoomID";
        public static String NumberOfRow = "NumberOfRow";
        public static String NumberOfColumn = "NumberOfColumn";

        //Bang booking
        public static String SeatRow = "Row";
        public static String SeatColumn = "Col";

        public static List<SelectListItem> listTech = getListTech();

        private static List<SelectListItem> getListTech()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            // Lay list role ra tu database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from [TechOfFilm]";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int RoleIDr = (int)reader[DatabaseHelper.TechID];
                        string RoleNamer = reader[DatabaseHelper.Feature].ToString();
                        listItems.Add(new SelectListItem() { Value = RoleIDr.ToString(), Text = RoleNamer });
                    }
                }
            }

            return listItems;
        }

        public static int getPriceByScheID(string ScheID)
        {
            int price = 0;
            return price;
        }


        public static string AutoGenerateCode()
        {
            string CharList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var t = DateTime.UtcNow;
            char[] charArray = CharList.ToCharArray();
            var result = new Stack<char>();

            var length = charArray.Length;

            long dgit = 1000000000000L +
                        t.Millisecond * 1000000000L +
                        t.DayOfYear * 1000000L +
                        t.Hour * 10000L +
                        t.Minute * 100L +
                        t.Second;

            while (dgit != 0)
            {
                result.Push(charArray[dgit % length]);
                dgit /= length;
            }
            return new string(result.ToArray());
        }

        public static string Home = "";
        public static string New = "";
        public static string Booking = "";
        public static string Contact = "";
        public static string Ticket = "";
        public static void setActiceMenu(string menu)
        {
            Home = "";
            New = "";
            Booking = "";
            Contact = "";
            Ticket = "";
            switch (menu)
            {
                case "Home":
                    Home = "active";
                    break;
                case "New":
                    New = "active";
                    break;
                case "Ticket":
                    Ticket = "active";
                    break;
                case "Booking":
                    Booking = "active";
                    break;
                case "Contact":
                    Contact = "active";
                    break;
            }
        }

        public static string getRoomByScheID(string ScheID)
        {
            string room = "";

            // Lay list role ra tu database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select Name from Room inner join Schedule 
                                        on Schedule.RoomID = Room.RoomID
                                        where ScheduleID=@ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ScheID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        room = reader[DatabaseHelper.Name].ToString();
                    }
                }
            }

            return room;
        }
    }
}