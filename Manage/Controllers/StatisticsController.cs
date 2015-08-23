using Manage.Models;
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

        private int getTotalSaleByDate(string dateSelect)
        {
            int total = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();
                string sqlSelect = @"Select Orders.totalPrice from Orders join Schedule
                                        on Orders.ScheduleId = Schedule.ScheduleID 
                                        where Schedule.DateSche = @date";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dateSelect);
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
                            <LabelStyle Font=""Verdana, 12px"" />
                          </AxisY>
                          <AxisX LineColor=""64, 64, 64, 64"" Interval=""1"">
                            <LabelStyle Font=""Verdana, 12px"" />
                          </AxisX>
                        </ChartArea>
                      </ChartAreas>
                    </Chart>";
            var model = getStaticsticDetail(year, month);
            var bytes = new Chart(width: model.width, height: model.height, theme: ChartTheme.Blue);
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
                        monthTotal += getTotalSaleByDate(dateSelect);
                    }

                    // gan vao bieu do
                    model.yValue[i - 1] = monthTotal.ToString();
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
                    model.yValue[i - 1] = getTotalSaleByDate(dateSelect).ToString();
                    model.xValue[i - 1] = i + "";
                }
            }

            return model;
        }

        public ActionResult FilmSale()
        {
            return View();
        }
    }
}
