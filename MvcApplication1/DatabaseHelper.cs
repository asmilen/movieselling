using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieSelling.Models;

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
        public static string dateSche = "DateSche";

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


        //Trang that dat ve
        public static String booked = "Đã đặt";
        public static String paid = "Đã thanh toán";

        public static List<SelectListItem> listTech = getListTech();
        public static int FilmSelected;

        public static List<SelectListItem> numberOfSeats = getlistSeat();

        private static List<SelectListItem> getlistSeat()
        {
            var numberOfSeat = new List<SelectListItem>();
            numberOfSeat.Add(new SelectListItem() { Text = "1", Value = "1" });
            numberOfSeat.Add(new SelectListItem() { Text = "2", Value = "2" });
            numberOfSeat.Add(new SelectListItem() { Text = "3", Value = "3" });
            numberOfSeat.Add(new SelectListItem() { Text = "4", Value = "4" });
            return numberOfSeat;
        }

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
            Schedule model = getScheduleByID(ScheID);

            // Dua vao gio chieu la ban ngay hay buoi toi va ngay chieu la cuoi tuan hay ngay thuong ma lay ra gia tri tuong ung 

            // Convert gio chieu sang dang int VD: 09:00 -> 900 ; 15:00 -> 1500
            int TimeInt = Int32.Parse(model.startTime.Replace(":", ""));

            // Neu thoi gian la tu 9h sang den 17h chieu thi DayOrNight = true neu khong DayOrNight = false;
            bool DayOrNight = (TimeInt >= 900 && TimeInt <= 1700);

            DateTime date = DateTime.ParseExact(model.dateSche, DateFormat, System.Globalization.CultureInfo.InvariantCulture);
            // Lay ra ngay thu may trong tuan theo ngay chieu 
            bool Weekend = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);

            // Neu la ngay thuong thi 

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from [TechOfFilm] inner join Film on TechOfFilm.[TechID]=Film.TechID where Film.FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", model.FilmID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (DayOrNight && Weekend) return (int)(reader["WeekendDayPrice"]);
                        if (!DayOrNight && Weekend) return (int)(reader["WeekendNightPrice"]);
                        if (DayOrNight && !Weekend) return (int)(reader["NormalDayPrice"]);
                        if (!DayOrNight && !Weekend) return (int)(reader["NormalNightPrice"]);
                    }
                }
            }
            return price;
        }

        public static string getFilmNamebyID(int FilmID)
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
        public static string Schedule = "";

        public static void setActiceMenu(string menu)
        {
            Home = "";
            New = "";
            Booking = "";
            Contact = "";
            Ticket = "";
            Schedule = "";
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
                case "Schedule":
                    Schedule = "active";
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

        public static Schedule getScheduleByID(string ScheID)
        {
            Schedule model = new Schedule();
            model.ScheduleID = ScheID;

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select Schedule.*,Film.Name as FilmName,Film.Picture,Room.Name as RoomName 
                                    from Schedule 
                                    left join Room on Schedule.RoomID = Room.RoomID
                                    join Film on Schedule.FilmID = Film.FilmID
                                    where ScheduleID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ScheID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.FilmName = reader["FilmName"].ToString();
                        model.RoomName = reader["RoomName"].ToString();
                        model.startTime = reader[DatabaseHelper.StartTime].ToString();
                        model.dateSche = reader[DatabaseHelper.dateSche].ToString();
                        model.FilmID = (int)reader[DatabaseHelper.FilmID];

                        if (reader[DatabaseHelper.Picture] != null)
                            model.Picture = (Byte[])reader[DatabaseHelper.Picture];
                    }
                }
            }
            return model;
        }


    }
}