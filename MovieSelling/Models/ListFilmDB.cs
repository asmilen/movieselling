using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace MovieSelling.Models
{
    public class ListFilmDB 
    {
        public List<Film> myListFilm {get; set;}
        public string errorMessage {get; set;}

        public ListFilmDB()
        {
            errorMessage = "";
            myListFilm = new List<Film>();
            try
            {
                using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * 
                        from Film ";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        //cmd.Parameters.AddWithValue("@searchString", concert);
                        conn.Open();
                        SqlDataReader test = cmd.ExecuteReader();
                        DataTable schemaTable = test.GetSchemaTable();
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