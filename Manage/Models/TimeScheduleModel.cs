using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class TimeScheduleModel
    {
        public int TimeID { get; set; }

        [Required]
        [Display (Name = "Giờ chiếu")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH-mm}", ApplyFormatInEditMode = true)]
        public DateTime TimeSchedule { get; set; }

        public TimeScheduleModel(int TimeID, DateTime TimeSchedule)
        {
            this.TimeID = TimeID;
            this.TimeSchedule = TimeSchedule;
           
        }

        public TimeScheduleModel()
        {

        }
    }
}