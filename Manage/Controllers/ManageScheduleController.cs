using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Controllers
{
    [Authorize(Roles = "Manager,Administrator")]
    public class ManageScheduleController : Controller
    {
        //
        // GET: /ManageSchedule/

        public ActionResult ViewSche(string currDate)
        {
            Schedule model = new Schedule();
            model.DateSche = DateTime.Now;
            model.listScheduleByFilm = getScheduleByDate(currDate);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewSche(Schedule model)
        {
            model.listScheduleByFilm = getScheduleByDate(model.DateSche.ToString(DatabaseHelper.DateFormat));
            return View(model);
        }

        private Dictionary<int, List<ScheduleDetail>> getScheduleByDate(string dateTime)
        {
            var model = new Dictionary<int, List<ScheduleDetail>>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * from Schedule where DateSche = @Date";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", dateTime);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int ScheID = (int)reader[DatabaseHelper.ScheduleID];
                        string StartTime = reader[DatabaseHelper.StartTime].ToString();
                        int RoomID = (int)reader[DatabaseHelper.RoomID];
                        int FilmID = (int)reader[DatabaseHelper.FilmID];

                        // Loc lich chieu theo phong chieu
                        if (model.ContainsKey(RoomID))
                            model[RoomID].Add(new ScheduleDetail(ScheID, FilmID, StartTime));
                        else
                        {
                            List<ScheduleDetail> temp = new List<ScheduleDetail>();
                            temp.Add(new ScheduleDetail(ScheID, FilmID, StartTime));
                            model.Add(RoomID, temp);
                        }
                    }
                }
            }
            return model;
        }

        public ActionResult Add()
        {
            ViewBag.StatusMessage = "";

            AddSchedule model = new AddSchedule();
            Session["date"] = DateTime.MaxValue;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddSchedule model, int ID)
        {
            ModelState.Clear();

            // Validate data
            // Ngay khong duoc nho hon nay hien tai
            if (model.DateSche < DateTime.Now)
            {
                ModelState.AddModelError("","Ngày được chọn phải lớn hơn ngày hiện tại");
            }

            // case 1 button Add
            if (ID == 1)
            if (ModelState.IsValid)
            {
                Session["date"] = model.DateSche;
                // Chon ra nhung phim co start date < date selected < end date
                DatabaseHelper.listFilm = getListFilmByDate(model.DateSche);

                model.listSchedule = new List<ScheduleDetail>();
                // Tao ra list lich chieu de select
                foreach (var item in DatabaseHelper.listTimes)
                {
                    ScheduleDetail temp = new ScheduleDetail(1, 1, item.Text);
                    model.listSchedule.Add(temp);
                }
            }

            // case 2 button Save
            if (ID == 2 && !String.IsNullOrEmpty(model.FilmID))
            {
                ViewBag.StatusMessage = "";
                foreach (var item in model.listSchedule)
                    if (item.selected)
                    {
                        // Add lịch chiếu vào database và trả về có thành công hay không
                        int row = AddScheduleToDatabase(item.startTime,item.RoomID,model.FilmID);

                        // Nếu không thành công thì hiển thị giờ không thành công 
                        if (row <= 0) ViewBag.StatusMessage += item.startTime + ",";
                    }
                if (String.IsNullOrEmpty(ViewBag.StatusMessage)) ViewBag.StatusMessage = "Add lịch chiếu thành công";
                else ViewBag.StatusMessage += "đã được xếp lịch, không thể thêm lịch chiếu mới cho giờ chiếu này";
            }
            return View(model);
        }

        private int AddScheduleToDatabase(string time, string room, string filmID)
        {
            int rowAffected = 0;
            //Insert into database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @" IF not EXISTS 
                        (SELECT * FROM Schedule WHERE RoomID = @RoomID and StartTime = @StartTime and DateSche = @DateSche ) 
                        Insert into Schedule values (@RoomID,@FilmID,@StartTime,@DateSche)";

                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            DateTime dateSche = (DateTime)Session["date"];
                            // Add value
                            cmd.Parameters.AddWithValue("@RoomID", room);
                            cmd.Parameters.AddWithValue("@FilmID", Int32.Parse(filmID));
                            cmd.Parameters.AddWithValue("@StartTime", time);
                            cmd.Parameters.AddWithValue("@DateSche", dateSche.ToString(DatabaseHelper.DateFormat));
                            // Exec
                            rowAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();
                conn.Dispose();
            }
            return rowAffected;
        }

        public static int getPriceByFilmID(string filmID,string time,DateTime dateSche)
        {
            // Dua vao gio chieu la ban ngay hay buoi toi va ngay chieu la cuoi tuan hay ngay thuong ma lay ra gia tri tuong ung 

            // Convert gio chieu sang dang int VD: 09:00 -> 900 ; 15:00 -> 1500
            int TimeInt = Int32.Parse(time.Replace(":",""));

            // Neu thoi gian la tu 9h sang den 17h chieu thi DayOrNight = true neu khong DayOrNight = false;
            bool DayOrNight = (TimeInt>= 900 && TimeInt <= 1700);

            // Lay ra ngay thu may trong tuan theo ngay chieu 
            bool Weekend = (dateSche.DayOfWeek == DayOfWeek.Saturday || dateSche.DayOfWeek == DayOfWeek.Sunday );

            // Neu la ngay thuong thi 
            
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from [TechOfFilm] inner join Film on TechOfFilm.[TechID]=Film.TechID where Film.FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", filmID);
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
            return 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStep2(AddSchedule model)
        {
            return View();
        }

        private List<SelectListItem> getListFilmByDate(DateTime DateSelected)
        {
            List<SelectListItem> mylist = new List<SelectListItem>();

            // Lay list film ra tu database voi start Date < DateSelected va DateSelected < EndDate
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from Film where StartDate < @date and @date < EndDate";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.Add("@date", System.Data.SqlDbType.DateTime).Value = DateSelected;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int RoleIDr = (int)reader[DatabaseHelper.FilmID];
                        string RoleNamer = reader[DatabaseHelper.Name].ToString();
                        mylist.Add(new SelectListItem() { Value = RoleIDr.ToString(), Text = RoleNamer });
                    }
                }
            }

            return mylist;
        }

        public ActionResult Delete(int ScheID)
        {
            string dateSche = "";
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select DateSche from Schedule where ScheduleID = @ID";
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ScheID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dateSche = reader[DatabaseHelper.DateSche].ToString();
                    }
                }
                conn.Close();
                conn.Open();
                sqlSelect = @"Delete from Schedule where ScheduleID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ScheID);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return RedirectToAction("ViewSche", new { currDate=dateSche });
        }

        public ActionResult Copy(DateTime fromDate,DateTime toDate)
        {
            if (fromDate != toDate)
            {
                int rowAffected = 0;
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    conn.Open();
                    string sqlSelect = @"Insert into Schedule (RoomID,FilmID,StartTime,DateSche)
                                    (Select RoomID , FilmID , StartTime , @toDate from Schedule where dateSche=@fromDate)";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@toDate", toDate.ToString(DatabaseHelper.DateFormat));
                        cmd.Parameters.AddWithValue("@fromDate", fromDate.ToString(DatabaseHelper.DateFormat));
                        rowAffected = cmd.ExecuteNonQuery();
                    }
                }
                ViewBag.StatusMessage = rowAffected + " lịch chiếu được sao chép thành công ";
            }
            else
            {
                ViewBag.StatusMessage = "";
            }
            return View();
        }
    }
}
