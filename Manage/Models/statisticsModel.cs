using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Models
{
    public class StatisticsDetail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string chartType { get; set; }
        public string[] xValue { get; set; }
        public string[] yValue { get; set; }

        public StatisticsDetail()
        {

        }

    }

    public class TotalSale
    {
        public string yearSelect { get; set; }
        public string monthSelect { get; set; }
        public List<SelectListItem> listYear {get;set;}
        public List<SelectListItem> listMonth { get; set; }

        public TotalSale(string year, string month)
        {
            listYear = new List<SelectListItem>();

            // Ho tro thong ke trong 3 nam
            var currDate = DateTime.UtcNow.AddHours(7);
            for (int i = 0; i < 3; i++)
            {
                listYear.Add(new SelectListItem() { Value = currDate.Year.ToString(), Text = currDate.Year.ToString() });
                currDate = currDate.AddYears(-1);
            }

            listMonth = new List<SelectListItem>();

            listMonth.Add(new SelectListItem() { Value = "0", Text = "Tất cả" , Selected = (month == "0")});

            for (int i = 1; i < 13; i++)
            {
                var monthInList = i.ToString();
                if (i < 10) monthInList = "0" + monthInList;

                listMonth.Add(new SelectListItem() { Value = monthInList, Text = "Tháng " + i });
            }

            yearSelect = year;
            monthSelect = month;
        }
    }

    public class FilmSale
    {
        public string FilmSelect { get; set; }
        public string monthSelect { get; set; }
        public List<SelectListItem> listFilm {get;set;}
        public List<SelectListItem> listMonth { get; set; }

        public FilmSale(string filmID, string month)
        {
            listMonth = new List<SelectListItem>();

            for (int i = 1; i < 13; i++)
            {
                var monthInList = i.ToString();
                if (i < 10) monthInList = "0" + monthInList;
                listMonth.Add(new SelectListItem() { Value = monthInList, Text = "Tháng " + i });
            }

            monthSelect = month;
        }
    }
}