using MovieSelling.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieSelling.Controllers
{
    public class ScheduleController : Controller
    {
        //
        // GET: /Schedule/

        public ActionResult Index()
        {
            DatabaseHelper.setActiceMenu("Schedule");
            Dictionary<string, Dictionary<int, List<string>>> model = getFromDatabase();
            return View(model);
        }

        private Dictionary<string, Dictionary<int, List<string>>> getFromDatabase()
        {
            Dictionary<string, Dictionary<int, List<string>>> model = new Dictionary<string, Dictionary<int, List<string>>>();
            DateTime now = DateTime.UtcNow.AddHours(7);
            for (int i = 0; i < 4; i++)
            {
                model.Add(now.ToString(DatabaseHelper.DateFormat), getScheduleByDate(now.ToString(DatabaseHelper.DateFormat)) );
                now = now.AddDays(1);
            }
            return model;
        }


        private Dictionary<int,List<string>> getScheduleByDate(string dateSche)
        {
            Dictionary<int, List<string>> myDic = new Dictionary<int, List<string>>();
            // 
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"select * from Schedule where [DateSche]=@date";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dateSche);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string startTime = reader[DatabaseHelper.StartTime].ToString();
                        int filmID = (int) reader[DatabaseHelper.FilmID];

                        if (myDic.ContainsKey(filmID))
                            myDic[filmID].Add(startTime);
                        else
                            myDic.Add(filmID, new List<string>() { startTime});
                    }
                }
            }

            foreach (var item in myDic)
            {
                item.Value.Sort();                        
            }
            return myDic;
        }
    }
}
