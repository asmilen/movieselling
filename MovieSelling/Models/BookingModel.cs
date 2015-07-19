using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieSelling.Models
{
    public class BookingModel
    {
        public List<SelectListItem> listFilm { get; set; }

        public List<SelectListItem> listDate { get; set; }

        public List<SelectListItem> listTime { get; set; }

        public List<SelectListItem> numberOfSeats { get; set; }

        [Required]
        public string filmSelected { get; set; }
        public string dateSelected { get; set; }
        public string ScheID { get; set; }

        public string seatSelected { get; set; }

        public BookingModel()
        {
            // Khoi tao list So ghe muon dat, toi da 4 ghe
            numberOfSeats = new List<SelectListItem>();
            numberOfSeats.Add(new SelectListItem() { Text = "1", Value = "1" });
            numberOfSeats.Add(new SelectListItem() { Text = "2", Value = "2" });
            numberOfSeats.Add(new SelectListItem() { Text = "3", Value = "3" });
            numberOfSeats.Add(new SelectListItem() { Text = "4", Value = "4" });

            // Khoi tao danh sach ngay de chon, chi cho dat ve trong hom nay va ngay mai
            listDate = new List<SelectListItem>();
            DateTime currDate = DateTime.Now;
            listDate.Add(new SelectListItem() { Text = currDate.ToString("dd-MM-yyyy"), Value = currDate.ToString("dd-MM-yyyy") });
            currDate = currDate.AddDays(1);
            listDate.Add(new SelectListItem() { Text = currDate.ToString("dd-MM-yyyy"), Value = currDate.ToString("dd-MM-yyyy") });

            listTime = new List<SelectListItem>();
            listFilm = new List<SelectListItem>();
        }

    }

    public class BookingStep2
    {
        public string FilmNameFull { get; set; }

        public string dateSche { get; set; }

        public string ScheID { get; set; }

        public string timeSche { get; set; }

        public bool[,] listSeat { get; set; }

        public string numberSeat { get; set; }
    }

    public class Ticket
    {
        public Customer customer { get; set; }

        public List<Seat> seat { get; set; }

        public string ScheID { get; set; }

    }

    public class Seat
    {
        public int Row {get;set;}
        public int Column {get;set;}

        public int Price { get; set; }
        public Seat()
        {

        }

        public Seat(int Row, int Column)
        {
            this.Row = Row;
            this.Column = Column;
        }

    }
}