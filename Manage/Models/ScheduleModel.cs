using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class Schedule
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime DateSche { get; set; }

        public Dictionary<Int32, List<ScheduleDetail>> listScheduleByFilm { get; set; }
    }

    public class ScheduleDetail
    {
        public string startTime { get; set; }
        public int ScheduleID { get; set; }
        public int RoomID { get; set; }

        public ScheduleDetail(int ScheduleID, int RoomID, string startTime)
        {
            this.startTime = startTime;
            this.ScheduleID = ScheduleID;
            this.RoomID = RoomID;
        }

    }
}