using MovieSelling.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace MovieSelling.Controllers
{
    public class BookingController : Controller
    {
        //
        // GET: /Booking/
        public ActionResult Step1()
        {
            ViewBag.SubMenu = "STEP 1: SELECT MOVIE ";
            
            // Tao model booking moi cho view
            BookingModel model = new BookingModel();

            // Lay ra List Film dang chieu
            model.listFilm = getListFilm(1);

            return View(model);
        }

        // getListFilm
        // Lay ra List Film dang chieu 
        // filmSelected 
        // Truyen vao gia tri select default cho list film
        private List<SelectListItem> getListFilm(int filmSelected)
        {
            List<SelectListItem> myListFilm = new List<SelectListItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from Film ";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int FilmID = (int)reader[DatabaseColumnName.FilmID];
                            string Name = reader[DatabaseColumnName.Name].ToString();
                            DateTime startDate = (DateTime) reader[DatabaseColumnName.StartDate];
                            DateTime endDate = (DateTime)reader[DatabaseColumnName.EndDate];
                            endDate = endDate.AddDays(1);

                            //Kiem tra neu Start Date < current Date < End Date thi moi add vao list
                            if (startDate < DateTime.Now && DateTime.Now < endDate)
                            {
                                // set gia tri selected theo gia tri truyen vao
                                bool selected = filmSelected == FilmID ;
                                myListFilm.Add(new SelectListItem() {Value=FilmID.ToString() , Text= Name , Selected = selected });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
            }
            return myListFilm;            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step1(BookingModel model)
        {
            ViewBag.SubMenu = "STEP 1: SELECT MOVIE ";

            // Lay ra List Film dang chieu
            model.listFilm = getListFilm(Int32.Parse(model.filmSelected));

            // Lay ra cac gio chieu 
            model.listTime = getListSchedule(0,Int32.Parse(model.filmSelected),model.dateSelected);

            return View(model);
        }

        // getListSchedule
        // Lay ra cac gio chieu theo film va ngay da chon
        // timeSelected 
        // Truyen vao gia tri select default cho list lich chieu
        private List<SelectListItem> getListSchedule(int timeSelected, int filmSelected, string dateSelected)
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    // Select trong bang schedule voi film va ngay da chon
                    string sqlSelect = @"select * from Schedule where FilmID=@id and DateSche=@date ";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@id",filmSelected);
                        cmd.Parameters.AddWithValue("@date", dateSelected);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int ScheID = (int)reader[DatabaseColumnName.ScheduleID];
                            string startTime = reader[DatabaseColumnName.StartTime].ToString();

                            bool selected = filmSelected == ScheID;
                            mylist.Add(new SelectListItem() { Value = ScheID.ToString(), Text = startTime, Selected = selected });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
            }
            return mylist;
        }

        //Step 2
        //Chon Ghe
        public ActionResult Step2(BookingModel model)
        {
            ViewBag.SubMenu = "STEP 2: SELECT SEAT";
            Session["time"] = DateTime.Now;
            Session["scheID"] = model.timeSelected; 
            return View();
        }

        //Step 3
        //Thanh toan
        public ActionResult Step3(string ListSeat)
        {
            //ViewBag.Message = Session["scheID"] + Session["time"].ToString();
            ViewBag.Message = ListSeat;
            return View();
        }
    }
}
