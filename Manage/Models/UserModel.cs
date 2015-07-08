using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class User
    {
        public int UserID;

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Required]
        [Display(Name = "Picture")]
        public byte[] picture { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "User name")]
        public string username { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public User (int UserID,string Name,DateTime DateOfBirth,byte[] Picture,string Address,string username)
        {
            this.UserID = UserID;
            this.Name = Name;
            this.DateOfBirth = DateOfBirth;
            this.picture = Picture;
            this.address = Address;
            this.username = username;
        }
        
        public User()
        {
        }
    }

    public class ViewUser
    {
        public int UserID;

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public string DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string address { get; set; }

        [Required]
        [Display(Name = "Picture")]
        public byte[] picture { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string username { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public ViewUser(int UserID, string Name, string DateOfBirth, byte[] Picture, string Address, string username)
        {
            this.UserID = UserID;
            this.Name = Name;
            this.DateOfBirth = DateOfBirth;
            this.picture = Picture;
            this.address = Address;
            this.username = username;
        }

        public ViewUser()
        {
            this.RoleName = "";
            this.picture = new byte[0];
            this.Name = "";
            this.username = "";
            this.address = "";
            this.DateOfBirth = "";
        }
    }
}