using MovieSelling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MovieSelling
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private ListFilmDB mylist = new ListFilmDB();
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty(mylist.errorMessage))
            {
                return View(mylist);
            }
            else
            {
                return View();
            }
        }
    }
}
