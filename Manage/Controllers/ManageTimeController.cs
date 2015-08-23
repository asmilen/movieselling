using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Controllers
{
    public class ManageTimeController : Controller
    {
        //
        // GET: /ManageTime/

        public ActionResult Index()
        {
            List<TimeScheduleModel> model = getListTime();
            return View(model);
        }

        private List<TimeScheduleModel> getListTime()
        {
            List<TimeScheduleModel> listItems = new List<TimeScheduleModel>();

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
                        listItems.Add(new TimeScheduleModel() { TimeID = TimeIDr , TimeSchedule = DateTime.ParseExact(Timer,"HH:mm",System.Globalization.CultureInfo.InvariantCulture) });
                    }
                }
            }

            return listItems;
        }

        //
        // GET: /ManageTime/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ManageTime/Create

        [HttpPost]
        public ActionResult Create(TimeScheduleModel model)
        {
            try
            {
                //Insert into database
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    conn.Open();
                    string sqlSelect = @"Insert into TimeSchedule values (@time)";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        // Add value
                        cmd.Parameters.AddWithValue("@time", model.TimeSchedule.ToString("HH:mm"));
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    conn.Dispose();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ManageTime/Delete/5

        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();

                // Delete from Schedule
                string sqlSelect = @"Delete from TimeSchedule Where TimeID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                conn.Dispose();
            }
            return RedirectToAction("Index");
        }
    }
}
