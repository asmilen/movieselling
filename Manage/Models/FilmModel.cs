using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Models
{
    public class FilmModel
    {
        public int FilmID { get; set; }
        public int UserID { get; set; }


        [Display(Name = "Category")]
        public List<SelectListItem> listCate { get; set; }

        [Display(Name = "Technology")]
        public List<SelectListItem> listTech { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryID { get; set; }

        [Required]
        [Display(Name = "Film Name")]
        public string Name { get; set; }

        [Display(Name = "Actor")]
        public string Actor { get; set; }

        [Display(Name = "Director")]
        public string Director { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Avatar Picture")]
        public byte[] Picture { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Tech")]
        public string TechID { get; set; }

        public FilmModel(int filmID, int UserId, string CategoryID , string Name , string Actor, string Director, string Description, byte[] Picture, DateTime StartDate, DateTime EndDate,string TechID)
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
            this.TechID = TechID;
        }
        public FilmModel()
        {
            // TODO: Complete member initialization
            this.FilmID = 0;
            this.UserID = 0;
            this.CategoryID = "";
            this.Name = "";
            this.Actor = "";
            this.Director = "";
            this.Description = "";
            this.Picture = new byte[0];
            this.StartDate = DateTime.MaxValue;
            this.EndDate = DateTime.MaxValue;
            this.listCate = new List<SelectListItem>();
            this.listTech = new List<SelectListItem>();
        }
    }
}