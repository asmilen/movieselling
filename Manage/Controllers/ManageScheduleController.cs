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
    public class ManageScheduleController : Controller
    {
        //
        // GET: /ManageSchedule/

        public ActionResult ViewSche()
        {
            Schedule model = new Schedule();
            model.DateSche = DateTime.Now;
            model.listScheduleByFilm = new Dictionary<int, List<ScheduleDetail>>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewSche(Schedule model)
        {
            model.listScheduleByFilm = new Dictionary<int, List<ScheduleDetail>>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * from Schedule where DateSche = @Date";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", model.DateSche.ToString(DatabaseHelper.DateFormat));
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int ScheID = (int)reader[DatabaseHelper.ScheduleID];
                        string StartTime = reader[DatabaseHelper.StartTime].ToString();
                        int RoomID = (int)reader[DatabaseHelper.RoomID];
                        int FilmID = (int)reader[DatabaseHelper.FilmID];

                        // Loc lich chieu theo phong chieu
                        if (model.listScheduleByFilm.ContainsKey(RoomID))
                            model.listScheduleByFilm[RoomID].Add(new ScheduleDetail(ScheID, FilmID, StartTime));
                        else
                        {
                            List<ScheduleDetail> temp = new List<ScheduleDetail>();
                            temp.Add(new ScheduleDetail(ScheID, FilmID, StartTime));
                            model.listScheduleByFilm.Add(FilmID, temp);
                        }
                    }
                }
            }
            return View(model);
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
                @ViewBag.StatusMessage = "Nếu room đã được xếp lịch trước đó, sẽ tự động bỏ qua và tiếp tục add những suất chiếu khác";
                foreach (var item in model.listSchedule)
                    if (item.selected)
                    {
                        AddScheduleToDatabase(item.startTime,item.FilmID,model.FilmID);  
                    }
            }
            return View(model);
        }

        private void AddScheduleToDatabase(string time, string room, string filmID)
        {
            //Insert into database
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @" IF not EXISTS 
                        (SELECT * FROM Schedule WHERE RoomID = @RoomID and StartTime = @StartTime and DateSche = @DateSche ) 
                        Insert into Schedule values (@RoomID,@FilmID,@StartTime,@DateSche)";

                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            // Add value
                            cmd.Parameters.AddWithValue("@RoomID", room);
                            cmd.Parameters.AddWithValue("@FilmID", Int32.Parse(filmID));
                            cmd.Parameters.AddWithValue("@StartTime", time);
                            cmd.Parameters.AddWithValue("@DateSche", ((DateTime)Session["date"]).ToString(DatabaseHelper.DateFormat));
                            // Exec
                            cmd.ExecuteNonQuery();
                }
                conn.Close();
                conn.Dispose();
            }
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
    }
}
