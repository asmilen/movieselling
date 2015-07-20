using MovieSelling.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieSelling.Controllers
{
    public class FilmController : Controller
    {
        //
        // GET: /Film/

        public ActionResult Index()
        {
            // Set active menu
            DatabaseHelper.setActiceMenu("Home");

            // Lay ra cac list phim
            FilmHome model = new FilmHome();

            model.listFilmTheater = getListFilm(1);
            model.listFilmComing = getListFilm(2);
            model.listFilmHot = getListFilm(3);

            return View(model);
        }

        public ActionResult ViewDetail(int FilmID)
        {
            return View();
        }

        public ActionResult OnTheater()
        {
            List<FilmView> mylist = getListFilm(1);
            return View(mylist);
        }

        //Case
        //    1: On Theater
        //    2: Coming Soon
        //    3: Phim Hot
        private List<FilmView> getListFilm(int Case)
        {
            List<FilmView> myListFilm = new List<FilmView>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from Film ";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        //cmd.Parameters.AddWithValue("@searchString", concert);
                        conn.Open();
                        SqlDataReader test = cmd.ExecuteReader();
                        while (test.Read())
                        {
                            int FilmID = (int)test[DatabaseHelper.FilmID];
                            string Name = test[DatabaseHelper.Name].ToString();

                            // Retrieve image
                            Byte[] data = new Byte[0];
                            data = (Byte[])(test[DatabaseHelper.Picture]);

                            myListFilm.Add(new FilmView(FilmID,Name,data));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
            }
            return myListFilm;
        }

        public ActionResult ComingSoon()
        {
            List<FilmView> mylist = getListFilm(2);
            return View(mylist);
        }
    }
}
