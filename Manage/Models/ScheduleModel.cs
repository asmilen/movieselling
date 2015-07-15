using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    public class AddSchedule
    {
        public string FilmID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime DateSche { get; set; }

        public List<ScheduleDetail> listSchedule { get; set; }

        public AddSchedule()
        {
            FilmID = "";
            DateSche = DateTime.MaxValue;
            listSchedule = new List<ScheduleDetail>();
        }

        public AddSchedule(string filmID, DateTime DateSche)
        {
            this.FilmID = filmID;
            this.DateSche = DateSche;
        }
    }
    public class ScheduleDetail
    {
        public string startTime { get; set; }
        public int ScheduleID { get; set; }
        public string RoomID { get; set; }

        public bool selected { get; set; }

        public ScheduleDetail(int ScheduleID, int RoomID, string startTime)
        {
            this.startTime = startTime;
            this.ScheduleID = ScheduleID;
            this.RoomID = RoomID.ToString();
        }

        public ScheduleDetail()
        {

        }
    }
}