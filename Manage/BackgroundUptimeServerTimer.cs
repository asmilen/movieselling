using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Hosting;

namespace Manage
{
    class BackgroundUptimeServerTimer : IRegisteredObject
    {
        private Timer _timer;

        public BackgroundUptimeServerTimer()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            // Set timer
            int delaytime = 0; // chạy ngay lập tức khi app được publish lên host
            int callbacktime = 2000; // chạy job mỗi 2000 milisecond = 2 second

            _timer = new Timer(InsertToDatabase, null, delaytime, callbacktime);
        }

        private void InsertToDatabase(object state)
        {
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"delete orders
                                     from orders 
                                     inner join schedule
                                     on schedule.ScheduleiD = orders.ScheduleiD
                                      where schedule.DateSche = @Date and @time > schedule.StartTime and orders.Status = @status";

                // cong them 1 tieng vao gio hien tai 
                var currDateTime = DateTime.UtcNow.AddHours(8);

                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", currDateTime.ToString(DatabaseHelper.DateFormat));
                    cmd.Parameters.AddWithValue("@time", currDateTime.Hour + ":" + currDateTime.Minute );
                    cmd.Parameters.AddWithValue("@status", DatabaseHelper.booked);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public void Stop(bool immediate)
        {
            _timer.Dispose();
            HostingEnvironment.UnregisterObject(this);
        }
    }
}
