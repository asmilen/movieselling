using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class TicketModel
    {
        
        public int TechID { get; set; }
        public string Feature { get; set; }

        [Required]
        public int WeekendDayPrice { get; set; }

        [Required]
        public int WeekendNightPrice { get; set; }

        [Required]
        public int NormalDayPrice { get; set; }

        [Required]
        public int NormalNightPrice { get; set; }

        [Required]
        public int Vip { get; set; }

        public TicketModel()
        {

        }
    }
    public class ListTicket
    {
        public List<TicketModel> mylist { get; set; }
        public ListTicket()
        {
            mylist = new List<TicketModel>();
        }
    }
}