using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieSelling.Models
{
    public class Schedule
    {
        public string startTime { get; set; }
        public string ScheduleID { get; set; }
        public string FilmName { get; set; }

        public string RoomName { get; set; }

        public string dateSche { get; set; }

        public byte[] Picture { get; set; }

        public Schedule()
        {

        }
    }
}