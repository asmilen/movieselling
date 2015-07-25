using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class RoomModel
    {
        public int RoomID { get; set; }

        [Required]
        [Display (Name = "Số Hàng Ghế ")]
        public string NumberOfRow { get; set; }

        [Required]
        [Display(Name = "Số Ghế Mỗi Hàng")]
        public string NumberOfColumn { get; set; }

        [Required]
        public string RoomName { get; set; }

        public RoomModel(int RoomID,string NumberOfRow,string NumberOfColumn,string RoomName)
        {
            this.RoomID = RoomID;
            this.NumberOfRow = NumberOfRow;
            this.NumberOfColumn = NumberOfColumn;
            this.RoomName = RoomName;
        }

        public RoomModel()
        {

        }
    }
}