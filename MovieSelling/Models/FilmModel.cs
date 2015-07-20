using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieSelling.Models
{
    public class FilmView
    {
        public int FilmID { get; set; }
        public string Name { get; set; }
        public byte[] Picture { get; set; }

        public FilmView(int FilmID,string Name, byte[] Picture)
        {
            this.FilmID = FilmID;
            this.Name = Name;
            this.Picture = Picture;
        }
    }

    public class Film 
    {
        public int FilmID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Actor { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Film(int filmID, int UserId, int CategoryID , string Name , string Actor, string Director, string Description, byte[] Picture, DateTime StartDate, DateTime EndDate)
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
            this.Picture = new byte[0];
            this.StartDate = DateTime.MaxValue;
            this.EndDate = DateTime.MaxValue;
        }
    }

    public class FilmHome
    {
        public List<FilmView> listFilmHot { get; set; }

        public List<FilmView> listFilmTheater { get; set; }

        public List<FilmView> listFilmComing { get; set; }

        public FilmHome()
        {
            listFilmHot = new List<FilmView>();
            listFilmTheater = new List<FilmView>();
            listFilmComing = new List<FilmView>();
        }
    }
}