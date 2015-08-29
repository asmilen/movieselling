using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieSelling2.Models
{
    public class HomeModel
    {
    }
    public class TicketPrice
    {

        public int TechID { get; set; }
        public string Feature { get; set; }

        public int WeekendDayPrice { get; set; }

        public int WeekendNightPrice { get; set; }

        public int NormalDayPrice { get; set; }

        public int NormalNightPrice { get; set; }

        public int Vip { get; set; }

        public TicketPrice()
        {

        }
    }
}