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


        [Display(Name = "Loại phim")]
        public List<SelectListItem> listCate { get; set; }

        [Display(Name = "Công nghệ")]
        public List<SelectListItem> listTech { get; set; }

        [Required]
        [Display(Name = "Thể loại")]
        public string CategoryID { get; set; }

        [Required]
        [Display(Name = "Tên phim")]
        public string Name { get; set; }

        [Display(Name = "Diễn viên")]
        public string Actor { get; set; }

        [Display(Name = "Đạo diễn")]
        public string Director { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Avatar Picture")]
        public byte[] Picture { get; set; }

        [Display(Name = "Picture 1")]
        public byte[] Picture1 { get; set; }

        [Display(Name = "Picture 2")]
        public byte[] Picture2 { get; set; }

        [Display(Name = "Picture 3")]
        public byte[] Picture3 { get; set; }

        [Display(Name = "Picture 4")]
        public byte[] Picture4 { get; set; }

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

        [Required]
        [Display(Name = "Thời Lượng")]
        public int filmLong { get; set; }

        [Required]
        [Display(Name = "Hãng Phim")]
        public string Company { get; set; }

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