using Manage.Models;
using System;
using System.Collections.Generic;
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
                    cmd.Parameters.AddWithValue("@Date", model.DateSche.ToString(DatabaseColumnName.DateFormat));
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int ScheID = (int)reader[DatabaseColumnName.ScheduleID];
                        string StartTime = reader[DatabaseColumnName.StartTime].ToString();
                        int RoomID = (int)reader[DatabaseColumnName.RoomID];
                        int FilmID = (int)reader[DatabaseColumnName.FilmID];

                        // Loc lich chieu theo phim
                        if (model.listScheduleByFilm.ContainsKey(FilmID))
                            model.listScheduleByFilm[FilmID].Add(new ScheduleDetail(ScheID, RoomID, StartTime));
                        else
                        {
                            List<ScheduleDetail> temp = new List<ScheduleDetail>();
                            temp.Add(new ScheduleDetail(ScheID, RoomID, StartTime));
                            model.listScheduleByFilm.Add(FilmID, temp);
                        }
                    }
                }
            }
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }
    }
}
