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
        [Required]
        public string filmSelected { get; set; }
        public string dateSelected { get; set; }
        public string ScheID { get; set; }

        public List<SelectListItem> listFilm { get; set; }

        public List<SelectListItem> listDate { get; set; }

        public List<SelectListItem> listTime { get; set; }
        public string seatSelected { get; set; }

        public BookingModel()
        {
            // Khoi tao list So ghe muon dat, toi da 4 ghe
            listFilm = new List<SelectListItem>();
            listDate = new List<SelectListItem>();
            listTime = new List<SelectListItem>();
        }

    }

    public class BookingStep2
    {
        public string FilmNameFull { get; set; }

        public string dateSche { get; set; }

        public string ScheID { get; set; }

        public string timeSche { get; set; }

        public int[,] listSeat { get; set; }

        public string numberSeat { get; set; }
    }

    public class Ticket
    {
        public Customer customer { get; set; }

        public List<Seat> seat { get; set; }

        public string ScheID { get; set; }

        public string code { get; set; }
        public string listseat { get; set; }

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