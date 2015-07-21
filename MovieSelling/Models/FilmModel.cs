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
        public string Category { get; set; }
        public string Name { get; set; }
        public string Actor { get; set; }
        public string Director { get; set; }
        public string Company { get; set; }

        public string Description { get; set; }
        public string filmLong { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Picture1 { get; set; }
        public byte[] Picture2 { get; set; }
        public byte[] Picture3 { get; set; }
        public byte[] Picture4 { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Film(int filmID, int UserId, string Category, string Name, string Actor, string Director, string Description, byte[] Picture, DateTime StartDate, DateTime EndDate, string Company, string FilmLong, byte[] Picture1, byte[] Picture2, byte[] Picture3, byte[] Picture4)
        {
            this.FilmID = filmID;
            this.UserID = UserID;
            this.Category = Category;
            this.Name = Name;
            this.Actor = Actor;
            this.Director = Director;
            this.Description = Description;
            this.Picture = Picture;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.Company = Company;
            this.filmLong = filmLong;
            this.Picture1 = Picture1;
            this.Picture2 = Picture2;
            this.Picture3 = Picture3;
            this.Picture4 = Picture4;
        }

        public Film()
        {
            // TODO: Complete member initialization
            this.FilmID = 0;
            this.UserID = 0;
            this.Category = "";
            this.Name = "";
            this.Actor = "";
            this.Director = "";
            this.Description = "";
            this.Company = "";
            this.filmLong = "";
            this.Picture = new byte[0];
            this.Picture1 = null;
            this.Picture2 = null;
            this.Picture3 = null;
            this.Picture4 = null;
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