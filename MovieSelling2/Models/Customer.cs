using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieSelling.Models
{
    public class Customer
    {

        public int CustomerID;

        [Required]
        [Display(Name = "Họ và Tên")]
        public string Name { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Chứng minh thư")]
        public string cmnd { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        public string phone { get; set; }

        public Customer (string Name,string address,string email,string cmnd,string phone)
        {
            this.Name = Name;
            this.address = address;
            this.email = email;
            this.cmnd = cmnd;
            this.phone = phone;
        }

        public Customer()
        {
        }
    }
}