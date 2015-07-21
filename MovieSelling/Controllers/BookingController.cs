﻿using MovieSelling.Models;
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
            DatabaseHelper.setActiceMenu("Booking");
            ViewBag.SubMenu = "BƯỚC 1: CHỌN PHIM";
            
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
                            int FilmID = (int)reader[DatabaseHelper.FilmID];
                            string Name = reader[DatabaseHelper.Name].ToString();
                            DateTime startDate = (DateTime) reader[DatabaseHelper.StartDate];
                            DateTime endDate = (DateTime)reader[DatabaseHelper.EndDate];
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
            ViewBag.SubMenu = "BƯỚC 1: CHỌN PHIM ";

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
                    string sqlSelect = @"select Schedule.ScheduleID,Schedule.StartTime,Room.NumberOfRow,Room.NumberOfColumn from Schedule inner join Room on Schedule.RoomID=Room.RoomID where FilmID=@id and DateSche=@date ";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@id",filmSelected);
                        cmd.Parameters.AddWithValue("@date", dateSelected);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int ScheID = (int)reader[DatabaseHelper.ScheduleID];
                            string startTime = reader[DatabaseHelper.StartTime].ToString();
                            
                            // Lay ra so ghe da dat
                            sqlSelect = @"select count(*) from Ticket join Orders on
                                            Ticket.OrderID = Orders.OrderID where ScheduleID=@Scheid";
                            using (SqlCommand cmd1 = new SqlCommand(sqlSelect, conn))
                            {
                                cmd1.Parameters.AddWithValue("@Scheid", ScheID);
                                Int32 count = (Int32)cmd1.ExecuteScalar();
                                startTime += " (" + count + "/" ;
                            }

                            // Tong so ghe
                            int TotalSeat = (int)reader[DatabaseHelper.NumberOfRow] * (int)reader[DatabaseHelper.NumberOfColumn];

                            startTime += TotalSeat + ")";

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
            // tao session moi
            ViewBag.SubMenu = "BƯỚC 2: CHỌN GHẾ";
            Session["time"] = DateTime.Now;
            Session["scheID"] = model.ScheID; 

            // Khoi tao gia tri cho model
            BookingStep2 modelStep2 = InitDataStep2(model);

            return View(modelStep2);
        }

        private BookingStep2 InitDataStep2(BookingModel model)
        {
            BookingStep2 modelStep2 = new BookingStep2();

            // Khoi tao gia tri cho ngay chieu, lich chieu , va tong so ghe
            modelStep2.dateSche = model.dateSelected;
            modelStep2.ScheID = model.ScheID;
            modelStep2.numberSeat = model.seatSelected;

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();

                // Select trong bang schedule,film,room lay ra gio chieu, so ghe trong phong va ten phim
                string sqlSelect = @"select Schedule.StartTime,Room.NumberOfRow,Room.NumberOfColumn , Film.Name
                                     from 
                                        Schedule left join Room on Schedule.RoomID=Room.RoomID 
                                        left join Film on Schedule.FilmID = Film.FilmID
                                        where ScheduleID=@id";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@id", modelStep2.ScheID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        modelStep2.FilmNameFull = reader[DatabaseHelper.Name].ToString();
                        modelStep2.timeSche = reader[DatabaseHelper.StartTime].ToString();
                        int rows = (int) reader[DatabaseHelper.NumberOfRow];
                        int columns = (int) reader[DatabaseHelper.NumberOfColumn];
                        modelStep2.listSeat = new bool[rows,columns];
                    }
                }

                // Khoi tao gia tri danh sach ghe da dat
                for (int i = 0; i < modelStep2.listSeat.GetLength(0); i++)
                    for (int j = 0; j < modelStep2.listSeat.GetLength(1); j++)
                        modelStep2.listSeat[i, j] = false;

                // Lay ra danh sach ghe da dat
                sqlSelect = @"select * from Ticket inner join Orders on Orders.OrderID=Ticket.OrderID where ScheduleID=@id";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@id", modelStep2.ScheID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int row = (int)reader[DatabaseHelper.SeatRow];
                        int column = (int)reader[DatabaseHelper.SeatColumn];
                        modelStep2.listSeat[row, column] = true;
                    }
                }
            }
            return modelStep2;
        }

        //Step 3
        //Thanh toan
        public ActionResult Step3(string ListSeat)
        {
            ViewBag.SubMenu = "BƯỚC 3: ĐIỀN THÔNG TIN NGƯỜI ĐẶT";

            // Tach Chuoi de lay ra ghe
            Ticket model = new Ticket();

            Session["listSeat"] = ListSeat;
            

            model.ScheID = Session["scheID"].ToString();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step3(Ticket model)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                // add khach hang vao database va lay ra Customer ID vua tao
                int CustomerID = InsertCustomerToDB(model.customer);

                // Lay ra danh sach ghe
                model.seat = getSeatFromList();

                // luu vao bang order
                int OrderID = InsertToOrder(model,CustomerID);

                // luu vao bang ticket_order
                foreach (var item in model.seat)
                {
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"Insert into Ticket values (@price,@OrderID,@Room,@row,@col)";
                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", OrderID);
                            cmd.Parameters.AddWithValue("@Room", DatabaseHelper.getRoomByScheID(model.ScheID));
                            cmd.Parameters.AddWithValue("@row", item.Row);
                            cmd.Parameters.AddWithValue("@col", item.Column);
                            cmd.Parameters.AddWithValue("@price", DatabaseHelper.getPriceByScheID(model.ScheID));

                            cmd.ExecuteScalar();
                            conn.Close();
                            conn.Dispose();
                        }
                    }
                }
            }
            return RedirectToAction("Step4");
        }

        public ActionResult Step4()
        {
            return View();
        }
        private List<Seat> getSeatFromList()
        {
            var temp = new List<Seat>();
            string ListSeat = Session["listSeat"].ToString();
            var seatTotal = Int32.Parse(ListSeat.Substring(0, 1));
            var index = 1;
            for (int i = 0; i < seatTotal; i++)
            {
                string seatI = ListSeat.Substring(index, 3);
                temp.Add(new Seat(Int32.Parse(seatI.Substring(0, 1)), Int32.Parse(seatI.Substring(2, 1))));
                index += 3;
            }
            return temp;
        }

        private int InsertToOrder(Ticket model, int CustomerID)
        {
            string code = DatabaseHelper.AutoGenerateCode();

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Insert into Orders output INSERTED.OrderID values (@CusID,@price,@numTicket,@ScheID,@code)";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@CusID", CustomerID);
                    cmd.Parameters.AddWithValue("@price", DatabaseHelper.getPriceByScheID(model.ScheID) * model.seat.Count);
                    cmd.Parameters.AddWithValue("@numTicket", model.seat.Count);
                    cmd.Parameters.AddWithValue("@ScheID", model.ScheID);
                    cmd.Parameters.AddWithValue("@code", code);

                    int modified = (int)cmd.ExecuteScalar();
                    conn.Close();
                    conn.Dispose();
                    return modified;
                }
            }
        }

        private int InsertCustomerToDB(Customer model)
        {
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Insert into Customer output INSERTED.CustomerID values (@Name,@add,@email,@cmnd,@phone)";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@add", model.address);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@cmnd", model.cmnd);
                    cmd.Parameters.AddWithValue("@phone", model.phone);

                    int modified = (int)cmd.ExecuteScalar();
                    conn.Close();
                    conn.Dispose();
                    return modified;
                }
            }
        }
    }
}
