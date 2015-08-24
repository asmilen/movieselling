﻿using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Manage.Controllers
{
    [Authorize(Roles = "Manager,Administrator")]
    public class StatisticsController : Controller
    {
        //
        // GET: /Statistics/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TotalSale(string year,string month)
        {
            TotalSale model = new TotalSale(year,month);
           
            return View(model);
        }

        private int getTotalSaleByDate(string dateSelect,int filmID)
        {
            int total = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select Orders.totalPrice from Orders join Schedule
                                        on Orders.ScheduleId = Schedule.ScheduleID 
                                        where Schedule.DateSche = @date ";
                if (filmID != 0) sqlSelect += "and Schedule.FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dateSelect);
                    cmd.Parameters.AddWithValue("@ID", filmID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int price = (int)reader[DatabaseHelper.TotalPrice];
                        total += price;
                    }
                }
            }
            return total;
        }

        public ActionResult Mychart(string year , string month)
        {
            string themeChart = @"<Chart>
                      <ChartAreas>
                        <ChartArea Name=""Default"" _Template_=""All"">
                          <AxisY>
                            <LabelStyle Font=""Arial, 14px"" />
                          </AxisY>
                          <AxisX LineColor=""64, 64, 64, 64"" Interval=""1"">
                            <LabelStyle Font=""Arial, 14px"" />
                            <MajorGrid Enabled =""False"" />
                          </AxisX>
                        </ChartArea>
                      </ChartAreas>
                    </Chart>";
            var model = getStaticsticDetail(year, month);
            var bytes = new Chart(width: model.width, height: model.height, theme: themeChart);
            if (month != "0")
            {
                bytes.AddTitle("Doanh số tháng " + month + "/" + year);
                bytes.SetXAxis(title: "ngày");
            }
            else
            {
                bytes.AddTitle("Doanh số năm " + year);
                bytes.SetXAxis(title: "Tháng");
            }
            bytes.AddSeries(chartType: model.chartType,
                            xValue: model.xValue, yValues: model.yValue);
            bytes.SetYAxis(title:"Doanh số");
            bytes.Write("png");
            return null;
        }

        private StatisticsDetail getStaticsticDetail(string year, string month)
        {
            var model = new StatisticsDetail();
            model.height = 600;
            model.width = 800;
            // Chọn tất cả
            if (month == "0")
            {
                model.xValue = new string[12];
                model.yValue = new string[12];
                model.chartType = "column";
                // Lay doanh so tung thang trong nam
                for (int i = 1; i < 13; i++)
                {
                    int monthTotal = 0;
                    string monthSelect = (i < 10) ? ("0" + i) : ("" + i) ;
                    // Lặp từng ngày trong tháng
                    for (int j = 1; j < 32; j++)
                    {
                        string dateSelect = j + "-" + monthSelect + "-" + year;
                        if (j < 10) dateSelect = "0" + dateSelect;
                        monthTotal += getTotalSaleByDate(dateSelect,0);
                    }

                    // gan vao bieu do
                    model.yValue[i - 1] = ConvertToStringWithSeparator(monthTotal);
                    model.xValue[i - 1] = i + "";
                }

            }
            // Chọn từng tháng
            else
            {
                model.xValue = new string[31];
                model.yValue = new string[31];
                model.chartType = "line";

                // Lặp từng ngày trong tháng
                for (int i = 1; i < 32; i++)
                {
                    string dateSelect = i + "-" + month + "-" + year;
                    if (i < 10) dateSelect = "0" + dateSelect;
                    model.yValue[i - 1] = getTotalSaleByDate(dateSelect,0).ToString();
                    model.xValue[i - 1] = i + "";
                }
            }

            return model;
        }

        private string ConvertToStringWithSeparator(int p)
        {
            string temp = p.ToString();
            int i = temp.Length;
            string ret = "";
            while (i > 0 )
            {
                if (i > 3) ret = "." + temp.Substring(i - 3, 3) + ret;
                else ret = temp.Substring(0, i);
                i -= 3;
            }
            return temp;
        }

        public ActionResult FilmSale(string FilmID,string month)
        {
            FilmSale model = new FilmSale(FilmID, month);
            model.listFilm = getListFilm(FilmID);
            model.FilmSelect = FilmID;
            return View(model);
        }

        public ActionResult FilmSaleChart(string filmID, string month,int type)
        {
            string themeChart = @"<Chart>
                      <ChartAreas>
                        <ChartArea Name=""Default"" _Template_=""All"">
                          <AxisY>
                            <LabelStyle Font=""Arial, 14px"" />
                          </AxisY>
                          <AxisX LineColor=""64, 64, 64, 64"" Interval=""2"">
                            <LabelStyle Font=""Arial, 14px"" />
                            <MajorGrid Enabled =""False"" />
                          </AxisX>
                        </ChartArea>
                      </ChartAreas>
                    </Chart>";
            var model = getStaticsticDetailByFilm(filmID, month, type);
            var year = DateTime.Now.Year;
            var bytes = new Chart(width: model.width, height: model.height, theme: themeChart);
            bytes.AddTitle("Doanh số tháng " + month + "/" + year);
            bytes.SetXAxis(title: "Ngày");

            bytes.AddSeries(chartType: model.chartType,
                            xValue: model.xValue, yValues: model.yValue);
            bytes.SetYAxis(title: "Doanh số");
            bytes.Write("png");
            return null;
        }

        private StatisticsDetail getStaticsticDetailByFilm(string filmID, string month, int type)
        {
            var model = new StatisticsDetail();
            model.height = 600;
            model.width = 800;
            // Chọn từng tháng
            model.xValue = new string[31];
            model.yValue = new string[31];
            model.chartType = "line";

            // Lặp từng ngày trong tháng
            for (int i = 1; i < 32; i++)
            {
                string dateSelect = i + "-" + month + "-" + DateTime.Now.Year;
                if (i < 10) dateSelect = "0" + dateSelect;
                if (type == 2) // Số vé
                {
                    model.yValue[i - 1] = getTotalTicketByDate(dateSelect, Int32.Parse(filmID)).ToString();
                }
                else // So Tien
                {
                    model.yValue[i - 1] = getTotalSaleByDate(dateSelect, Int32.Parse(filmID)).ToString();
                }
                model.xValue[i - 1] = i + "";
            }

            return model;
        }

        private object getTotalTicketByDate(string dateSelect, int filmID)
        {
            int total = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select Orders.numberofticket from Orders join Schedule
                                        on Orders.ScheduleId = Schedule.ScheduleID 
                                        where Schedule.DateSche = @date and Schedule.FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dateSelect);
                    cmd.Parameters.AddWithValue("@ID", filmID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int price = (int)reader["numberofticket"];
                        total += price;
                    }
                }
            }
            return total;
        }

        public ActionResult TicketSale(string FilmID, string month)
        {
            FilmSale model = new FilmSale(FilmID, month);
            model.listFilm = getListFilm(FilmID);
            model.FilmSelect = FilmID;
            return View(model);
        }

        private List<SelectListItem> getListFilm(string FilmSelect)
        {
            string sqlSelect = @"select FilmID,Name,Picture from Film ";
            sqlSelect += "Where '" + DateTime.UtcNow.AddHours(7) + "'<EndDate and '" + DateTime.UtcNow.AddHours(7) + "'>StartDate";

            List<SelectListItem> myListFilm = new List<SelectListItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader test = cmd.ExecuteReader();
                        while (test.Read())
                        {
                            string FilmID = test[DatabaseHelper.FilmID].ToString();
                            string Name = test[DatabaseHelper.Name].ToString();
                            myListFilm.Add(new SelectListItem() { Value = FilmID, Text = Name , Selected = (FilmID == FilmSelect)});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
                ViewBag.Message = ex.Message;
            }
            return myListFilm;
        }
    }
}
