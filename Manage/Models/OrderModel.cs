using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manage.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }
        public int ScheID { get; set; }
        public string CustomerEmail { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }
        public int TotalPrice { get; set; }
        public string listTicket { get; set; }
        public string code { get; set; }
        public string status { get; set; }

        public OrderModel()
        {

        }

        public OrderModel(int OrderID,string CustomerEmail,string CustomerName,string CustomerPhone,int TotalPrice,string listTicket,string code,string status,int ScheID)
        {
            this.OrderID = OrderID;
            this.CustomerEmail = CustomerEmail;
            this.CustomerName = CustomerName;
            this.CustomerPhone = CustomerPhone;
            this.TotalPrice = TotalPrice;
            this.listTicket = listTicket;
            this.code = code;
            this.status = status;
            this.ScheID = ScheID;
        }
    }
}