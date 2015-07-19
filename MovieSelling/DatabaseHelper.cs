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
        public static String SeatRow = "SeatRow";
        public static String SeatColumn = "SeatColumn";

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

            // Lay list role ra tu database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select Price from Schedule where ScheduleID=@ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ScheID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        price = (int)reader[DatabaseHelper.Price];
                    }
                }
            }

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
    }
}