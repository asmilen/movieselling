using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Controllers
{
    public class ManageOrderController : Controller
    {
        //
        // GET: /ManageOrder/

        public ActionResult Index(int ScheID)
        {
            List<OrderModel> model = getOrderByScheID(ScheID);
            return View(model);
        }

        private List<OrderModel> getOrderByScheID(int ScheID)
        {
            List<OrderModel> model = new List<OrderModel>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"select * from Orders 
                                      inner join Customer on Orders.CustomerID = Customer.CustomerID  
                                      where ScheduleID=@ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", ScheID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int OrderID = (int)reader[DatabaseHelper.OrderID];
                        string CusName = reader[DatabaseHelper.Name].ToString();
                        string CusEmail = reader[DatabaseHelper.Email].ToString();
                        string CusPhone = reader[DatabaseHelper.Phone].ToString();
                        int TotalPrice = (int)reader[DatabaseHelper.TotalPrice];
                        string code = reader["code"].ToString();
                        string listTicket = getListTicket(OrderID);
                        string status = reader["Status"].ToString();
                        model.Add(new OrderModel(OrderID, CusEmail, CusName, CusPhone, TotalPrice, listTicket, code, status, ScheID));
                    }
                }
            }
            return model;
        }

        private string getListTicket(int OrderID)
        {
            string model = "";
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"select * from Ticket 
                                      where OrderID=@ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", OrderID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int row = (int)reader["Row"];
                        int col = (int)reader["Col"];
                        model += Char.ConvertFromUtf32(row + 65) + "-" + col + " ";
                    }
                }
            }
            return model;
        }

        public ActionResult Change(int OrderID)
        {
            // Lay ra status
            string status = ""; int ScheID = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();

                // Lat ra status hien tai
                string sqlSelect = @"Select ScheduleID,status from Orders where OrderID=@ID";
                SqlCommand cmd = new SqlCommand(sqlSelect, conn);
                cmd.Parameters.AddWithValue("@ID", OrderID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    status = reader["Status"].ToString();
                    ScheID = (int) reader[DatabaseHelper.ScheduleID];   
                }
                conn.Close();
                // Thay doi status
                if (status.Equals(DatabaseHelper.booked)) status = DatabaseHelper.paid;
                else status = DatabaseHelper.booked;

                //Update vao database
                sqlSelect = @"update Orders set Status=@sta where OrderID=@ID";
                cmd = new SqlCommand(sqlSelect, conn);
                cmd.Parameters.AddWithValue("@ID", OrderID);
                cmd.Parameters.AddWithValue("@sta",status);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return RedirectToAction("Index",new {ScheID});
        }

    }
}

