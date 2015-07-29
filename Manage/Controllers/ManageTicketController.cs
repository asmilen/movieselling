using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Controllers
{
    [Authorize(Roles = "Manager,Administrator")]
    public class ManageTicketController : Controller
    {
        //
        // GET: /ManageTicket/

        public ActionResult Index()
        {
            ListTicket mylist = new ListTicket();
            mylist.mylist = getCurrentPrice();
            return View(mylist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ListTicket model)
        {
            foreach (var item in model.mylist)
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Update TechOfFilm Set [NormalDayPrice]=@NMP,[NormalNightPrice]=@NNP,
                                                           [WeekendDayPrice]=@WDP,[WeekendNightPrice]=@WNP, Vip =@Vip
                                        where TechID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", item.TechID);
                    cmd.Parameters.AddWithValue("@NMP", item.NormalDayPrice);
                    cmd.Parameters.AddWithValue("@NNP", item.NormalNightPrice);
                    cmd.Parameters.AddWithValue("@WDP", item.WeekendDayPrice);
                    cmd.Parameters.AddWithValue("@WNP", item.WeekendNightPrice);
                    cmd.Parameters.AddWithValue("@Vip", item.Vip);

                    cmd.ExecuteNonQuery();
                }
            }
            }
            return View(model);
        }

        private List<TicketModel> getCurrentPrice()
        {
            List<TicketModel> mylist = new List<TicketModel>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select * from [TechOfFilm]";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TicketModel model = new TicketModel();
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

    }
}
