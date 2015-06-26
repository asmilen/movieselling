using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieSelling.Models
{
    public class Film 
    {
        public int FilmID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Actor { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Film(int filmID, int UserId, int CategoryID , string Name , string Actor, string Director, string Description, string Picture, DateTime StartDate, DateTime EndDate)
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
            this.Picture = "";
            this.StartDate = DateTime.MaxValue;
            this.EndDate = DateTime.MaxValue;
        }
    }

    public class ListFilmDB
    {
        public List<Film> myListFilm { get; set; }
        public string errorMessage { get; set; }

        public ListFilmDB()
        {
            errorMessage = "";
            myListFilm = new List<Film>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * 
                        from Film ";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        //cmd.Parameters.AddWithValue("@searchString", concert);
                        conn.Open();
                        SqlDataReader test = cmd.ExecuteReader();
                        while (test.Read())
                        {
                            int FilmID = (int)test[DatabaseColumnName.FilmID];
                            int UserID = (int)test[DatabaseColumnName.UserID];
                            int CategoryID = (int)test[DatabaseColumnName.CategoryID];
                            string Name = test[DatabaseColumnName.Name].ToString();
                            string Actor = test[DatabaseColumnName.Actor].ToString();
                            string Director = test[DatabaseColumnName.Director].ToString();
                            string Description = test[DatabaseColumnName.Description].ToString();
                            // Retrieve image
                            //Byte[] data = new Byte[0];
                            //data = (Byte[])(test[DatabaseColumnName.Picture]);
                            //MemoryStream mem = new MemoryStream(data);

                            DateTime StartDate = (DateTime)test[DatabaseColumnName.StartDate];
                            DateTime EndDate = (DateTime)test[DatabaseColumnName.EndDate];

                            myListFilm.Add(new Film(FilmID, UserID, CategoryID, Name, Actor, Director, Description, "", StartDate, EndDate));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + "\n" + ex.StackTrace;
            }
        }
    }
}