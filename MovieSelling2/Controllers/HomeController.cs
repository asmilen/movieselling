using MovieSelling2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieSelling.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Ticket()
        {
            DatabaseHelper.setActiceMenu("Ticket");
            List<TicketPrice> model = getListTicketPrice(); 
            return View(model);
        }

        private List<TicketPrice> getListTicketPrice()
        {
            List<TicketPrice> mylist = new List<TicketPrice>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from [TechOfFilm]";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TicketPrice model = new TicketPrice();
                        model.TechID = (int)(reader[DatabaseHelper.TechID]);
                        model.WeekendDayPrice = (int)(reader["WeekendDayPrice"]);
                        model.WeekendNightPrice = (int)(reader["WeekendNightPrice"]);
                        model.NormalDayPrice = (int)(reader["NormalDayPrice"]);
                        model.NormalNightPrice = (int)(reader["NormalNightPrice"]);
                        model.Vip = (int)(reader["Vip"]);
                        model.Feature = reader[DatabaseHelper.Feature].ToString();
                        mylist.Add(model);
                    }
                }
            }

            return mylist;
        }


        public ActionResult News()
        {
            DatabaseHelper.setActiceMenu("New");
            return View();
        }

        public ActionResult Contact()
        {
            DatabaseHelper.setActiceMenu("Contact");
            return View();
        }
    }
}
