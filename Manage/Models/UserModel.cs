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
        [Display(Name = "Role")]
        public int RoleID { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth;

        [Required]
        [Display(Name = "Address")]
        public string address;

        public Bitmap picture;

        [Required]
        [Display(Name = "User name")]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public User (int UserID,int RoleID,string Name,DateTime DateOfBirth,Bitmap Picture,string Address,string username)
        {
            this.UserID = UserID;
            this.RoleID = RoleID;
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
}