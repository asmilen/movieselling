using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieSelling.Models
{
    public class Film 
    {
        public int FilmID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Actor { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Film(int filmID, int UserId, int CategoryID , string Name , string Actor, string Director, string Description, string Picture, DateTime StartDate, DateTime EndDate)
        {
            this.FilmID = filmID;
            this.UserID = UserID;
            this.CategoryID = CategoryID;
            this.Name = Name;
            this.Actor = Actor;
            this.Director = Director;
            this.Description = Description;
            this.Picture = Picture;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
        }

        public Film()
        {
            // TODO: Complete member initialization
            this.FilmID = 0;
            this.UserID = 0;
            this.CategoryID = 0;
            this.Name = "";
            this.Actor = "";
            this.Director = "";
            this.Description = "";
            this.Picture = "";
            this.StartDate = DateTime.MaxValue;
            this.EndDate = DateTime.MaxValue;
        }
    }
}