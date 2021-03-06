﻿using MovieSelling.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieSelling.Controllers
{
    public class FilmController : Controller
    {
        //
        // GET: /Film/

        public ActionResult Index()
        {
            // Set active menu
            DatabaseHelper.setActiceMenu("Home");

            // Lay ra cac list phim
            FilmHome model = new FilmHome();

            model.listFilmTheater = getListFilm(1);
            model.listFilmComing = getListFilm(2);
            model.listFilmHot = getListFilm(3);

            return View(model);
        }

        public ActionResult ViewDetail(int FilmID)
        {
            FilmDetail film = new FilmDetail();

            film.filmDetail = getFilmByID(FilmID);

            film.filmSchedule = getScheduleByFilmID(FilmID);
            return View(film);
        }

        private Dictionary<string, List<string>> getScheduleByFilmID(int FilmID)
        {
            Dictionary<string, List<string>> schedule = new Dictionary<string, List<string>>();
            DateTime now = DateTime.UtcNow.AddHours(7);
            for (int i = 0; i < 4; i++)
            {
                // lay ra lich chieu film theo ngay
                string dateSche = now.ToString(DatabaseHelper.DateFormat);
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    conn.Open();
                    string sqlSelect = @"select * from Schedule where [DateSche]=@date and filmID=@ID";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", dateSche);
                        cmd.Parameters.AddWithValue("@ID", FilmID);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string startTime = reader[DatabaseHelper.StartTime].ToString();
                            int filmID = (int)reader[DatabaseHelper.FilmID];

                            if (schedule.ContainsKey(dateSche))
                                schedule[dateSche].Add(startTime);
                            else
                                schedule.Add(dateSche, new List<string>() { startTime });
                        }
                    }
                    now = now.AddDays(1);
                }
            }
            return schedule;
        }

        private Film getFilmByID(int FilmID)
        {
            Film temp = new Film();
            temp.FilmID = FilmID;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select Film.* , CategoryFilm.Name as CateName from Film join [CategoryFilm] on [CategoryFilm].[CategoryID]= Film.[CategoryID] where FilmID=@ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID", FilmID);
                    SqlDataReader test = cmd.ExecuteReader();
                    while (test.Read())
                    {
                            temp.UserID = (int)test[DatabaseHelper.UserID];
                            temp.Category = test["CateName"].ToString();
                            temp.Name = test[DatabaseHelper.Name].ToString();

                            // Vi Actor,director,Description co the null nen phai kiem tra
                            if (test[DatabaseHelper.Actor] != null)
                                temp.Actor = test[DatabaseHelper.Actor].ToString();

                            if (test[DatabaseHelper.Director] != null)
                                temp.Director = test[DatabaseHelper.Director].ToString();

                            if (test[DatabaseHelper.Company] != null)
                                temp.Company = test[DatabaseHelper.Company].ToString();

                            if (test[DatabaseHelper.filmLong] != null)
                                temp.filmLong = test[DatabaseHelper.filmLong].ToString();

                            if (test[DatabaseHelper.Description] != null)
                                temp.Description = test[DatabaseHelper.Description].ToString();

                            temp.StartDate = (DateTime)test[DatabaseHelper.StartDate];
                            temp.EndDate = (DateTime)test[DatabaseHelper.EndDate];

                            // Retrieve image
                            if (test[DatabaseHelper.Picture] != null)
                                temp.Picture = (Byte[])(test[DatabaseHelper.Picture]);
                            // Retrieve image
                            if (test[DatabaseHelper.Picture1] != null)
                                temp.Picture1 = (Byte[])(test[DatabaseHelper.Picture1]);
                            // Retrieve image
                            if (test[DatabaseHelper.Picture2] != null)
                                temp.Picture2 = (Byte[])(test[DatabaseHelper.Picture2]);
                            // Retrieve image
                            if (test[DatabaseHelper.Picture] != null)
                                temp.Picture3 = (Byte[])(test[DatabaseHelper.Picture3]);
                            // Retrieve image
                            if (test[DatabaseHelper.Picture4] != null)
                                temp.Picture4 = (Byte[])(test[DatabaseHelper.Picture4]);
                       
                    }
                }
            }
            return temp;
        }

        private int getTotalSaleByDate(string dateSelect, int filmID)
        {
            int total = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select Orders.totalPrice from Orders join Schedule
                                        on Orders.ScheduleId = Schedule.ScheduleID 
                                        where Schedule.DateSche = @date ";
                if (filmID != 0) sqlSelect += "and Schedule.FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dateSelect);
                    cmd.Parameters.AddWithValue("@ID", filmID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int price = (int)reader["TotalPrice"];
                        total += price;
                    }
                }
            }
            return total;
        }

        public ActionResult OnTheater()
        {
            List<FilmView> mylist = getListFilm(1);
            return View(mylist);
        }

        //Case
        //    1: On Theater
        //    2: Coming Soon
        //    3: Phim Hot
        private List<FilmView> getListFilm(int Case)
        {
            string sqlSelect = @"select FilmID,Name,Picture from Film ";
            switch (Case)
            {
                case 1:
                    sqlSelect += "Where '" + DateTime.UtcNow.AddHours(7) + "'<EndDate and '" + DateTime.UtcNow.AddHours(7) + "'>StartDate";
                    break;
                case 2:
                    sqlSelect += "Where '" + DateTime.UtcNow.AddHours(7) + "'<StartDate";
                    break;
                case 3:
                    sqlSelect += "Where '" + DateTime.UtcNow.AddHours(7) + "'<EndDate and '" + DateTime.UtcNow.AddHours(7) + "'>StartDate";
                    break;
            }

            List<FilmView> myListFilm = new List<FilmView>();
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        //cmd.Parameters.AddWithValue("@searchString", concert);
                        conn.Open();
                        SqlDataReader test = cmd.ExecuteReader();
                        while (test.Read())
                        {
                            int FilmID = (int)test[DatabaseHelper.FilmID];
                            string Name = test[DatabaseHelper.Name].ToString();

                            // Retrieve image
                            Byte[] data = new Byte[0];
                            data = (Byte[])(test[DatabaseHelper.Picture]);

                            myListFilm.Add(new FilmView(FilmID,Name,data));
                        }
                    }
                }

            // Sap xep phim hot trong tuan
                if (Case==3)
                {
                    foreach (var item in myListFilm)
                    {
                        item.totalSale = GetTotalSaleInWeek(item.FilmID);                            
                    }
                    return myListFilm.OrderByDescending(x => x.totalSale).ToList();
                }
            return myListFilm;
        }


        private int GetTotalSaleInWeek(int FilmID)
        {
            int total = 0;
            var currDate = DateTime.UtcNow.AddHours(7).AddDays(-6);
            for (int i = 0; i < 8; i++)
            {
                string Day = (currDate.Day < 10) ? ("0" + currDate.Day) : (currDate.Day.ToString());
                string Month = (currDate.Month < 10) ? ("0" + currDate.Month) : (currDate.Month.ToString());
                string dateSelect = Day + "-" + Month + "-" + DateTime.Now.Year;
                total += getTotalSaleByDate(dateSelect, FilmID);
                currDate = currDate.AddDays(1);
            }
            return total;
        }

        public ActionResult ComingSoon()
        {
            List<FilmView> mylist = getListFilm(2);
            return View(mylist);
        }
    }
}
