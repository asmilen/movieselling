using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Controllers
{
    [Authorize]
    public class ManageFilmController : Controller
    {
        //
        // GET: /ManageFilm/
        public ActionResult List()
        {
            ViewBag.Message = "Danh sách Film";
            List<FilmModel> mylist = getListFilm();
            return View(mylist);
        }

        private List<FilmModel> getListFilm()
        {
            List<FilmModel> templist = new List<FilmModel>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * from Film ";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    conn.Open();
                    SqlDataReader test = cmd.ExecuteReader();
                    while (test.Read())
                    {
                        int FilmID = (int)test[DatabaseHelper.FilmID];
                        int UserID = (int)test[DatabaseHelper.UserID];
                        string CateID = test[DatabaseHelper.CategoryID].ToString();
                        string Name = test[DatabaseHelper.Name].ToString();
                        string TechID = test[DatabaseHelper.TechID].ToString();

                        // Vi Actor,director,Description co the null nen phai kiem tra
                        string Actor = "";
                        if (test[DatabaseHelper.Actor] != null)
                            Actor = test[DatabaseHelper.Actor].ToString();

                        string Direc = "";
                        if (test[DatabaseHelper.Director] != null)
                            Direc = test[DatabaseHelper.Director].ToString();

                        string Desc = "";
                        if (test[DatabaseHelper.Description] != null)
                            Desc = test[DatabaseHelper.Description].ToString();

                        DateTime StartDate = (DateTime)test[DatabaseHelper.StartDate];
                        DateTime EndDate = (DateTime)test[DatabaseHelper.EndDate];

                        byte[] data = new byte[0];
                        templist.Add(new FilmModel(FilmID, UserID, CateID, Name, Actor, Direc, Desc, data, StartDate, EndDate, TechID));
                    }
                }
            }
            return templist;
        }

        public ActionResult Add()
        {
            FilmModel model = new FilmModel();
            model.listCate = getListCate();
            model.listTech = getListTech();
            return View(model);
        }

        private List<SelectListItem> getListCate()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from CategoryFilm";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int FilmID = (int)reader[DatabaseHelper.CategoryID];
                            string Name = reader[DatabaseHelper.Name].ToString();
                            mylist.Add(new SelectListItem() { Value = FilmID.ToString(), Text = Name });
                        }
                    }
                }
            mylist[0].Selected = true;
            return mylist;
        }

        private List<SelectListItem> getListTech()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from TechOfFilm";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int TechID = (int)reader[DatabaseHelper.TechID];
                            string Name = reader[DatabaseHelper.Feature].ToString();
                            mylist.Add(new SelectListItem() { Value = TechID.ToString(), Text = Name });
                        }
                    }
                }
            return mylist;
        }

        public ActionResult Edit(int FilmID)
        {
            var model = getFilmByID(FilmID);
            model.listCate = getListCate();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FilmModel model)
        {
            // Edit exist User to Database
            model.listCate = getListCate();
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    int UserID = WebMatrix.WebData.WebSecurity.CurrentUserId;
                    //Update vao database
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"update film set Name = @name, CategoryId = @cateID, Actor = @Actor, Director = @Director, Description = @Description, 
                                            StartDate = @StartDate, EndDate = @EndDate, Company = @Company, filmLong = @filmLong,UserID = @userid where FilmID = @filmID ";
                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", model.Name);
                            cmd.Parameters.AddWithValue("@cateID", model.CategoryID);
                            cmd.Parameters.AddWithValue("@Actor", model.Actor);
                            cmd.Parameters.AddWithValue("@Director", model.Director);
                            cmd.Parameters.AddWithValue("@Description", model.Description);
                            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                            cmd.Parameters.AddWithValue("@Company", model.Company);
                            cmd.Parameters.AddWithValue("@filmLong", model.filmLong);
                            cmd.Parameters.AddWithValue("@filmID", model.FilmID);
                            cmd.Parameters.AddWithValue("@userid", UserID);
                            cmd.ExecuteNonQuery();
                        }

                        conn.Close();
                        // update anh
                        for (int i = 0; i < 5; i++)
                        {
                            conn.Open();
                            // lay file
                            byte[] img = null;
                            string filename = "file";
                            if (i != 0) filename += i;
                            HttpPostedFileBase file = Request.Files[filename];

                            if (file != null && file.ContentLength > 0)
                            {
                                img = new byte[file.ContentLength];
                                file.InputStream.Read(img, 0, file.ContentLength);
                            }
                            if (img != null)
                            {
                                if (i == 0 )
                                    sqlSelect = @"update Film set Picture = @img where FilmID = @FilmID";
                                else
                                    sqlSelect = @"update Film set Picture"+i+" = @img where FilmID = @FilmID";
                                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                                {
                                    cmd.Parameters.AddWithValue("@FilmID", model.FilmID);
                                    cmd.Parameters.Add("@img", SqlDbType.VarBinary).Value = img;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", ModelState.Values.All(modelState => modelState.Errors.Count == 0).ToString());
            }
            return RedirectToAction("List");
        }

        public ActionResult ViewDetail(int FilmID)
        {
            var model = getFilmByID(FilmID);
            return View(model);
        }

        private FilmModel getFilmByID(int FilmID)
        {
            FilmModel temp = new FilmModel();
            temp.FilmID = FilmID;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * from Film  where FilmID=@ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID", FilmID);
                    SqlDataReader test = cmd.ExecuteReader();
                    while (test.Read())
                    {
                        if (test[DatabaseHelper.filmLong] != null)
                            temp.filmLong = (Int32.Parse((test[DatabaseHelper.filmLong].ToString().Trim())));

                        temp.UserID = (int)test[DatabaseHelper.UserID];
                        temp.CategoryID = test[DatabaseHelper.CategoryID].ToString();
                        temp.Name = test[DatabaseHelper.Name].ToString();

                        // Vi Actor,director,Description co the null nen phai kiem tra
                        if (test[DatabaseHelper.Actor] != null)
                            temp.Actor = test[DatabaseHelper.Actor].ToString();

                        if (test[DatabaseHelper.Director] != null)
                            temp.Director = test[DatabaseHelper.Director].ToString();

                        if (test[DatabaseHelper.Company] != null)
                            temp.Company = test[DatabaseHelper.Company].ToString();


                        if (test[DatabaseHelper.Description] != null)
                            temp.Description = test[DatabaseHelper.Description].ToString();

                        temp.StartDate = (DateTime)test[DatabaseHelper.StartDate];
                        temp.EndDate = (DateTime)test[DatabaseHelper.EndDate];

                        // Retrieve image
                        if (test[DatabaseHelper.Picture] != null)
                            temp.Picture = (Byte[])(test[DatabaseHelper.Picture]);
                        // Retrieve image
                        if (test[DatabaseHelper.Picture1] != null)
                            temp.Picture1 = (Byte[])(test[DatabaseHelper.Picture1]);
                        // Retrieve image
                        if (test[DatabaseHelper.Picture2] != null)
                            temp.Picture2 = (Byte[])(test[DatabaseHelper.Picture2]);
                        // Retrieve image
                        if (test[DatabaseHelper.Picture3] != null)
                            temp.Picture3 = (Byte[])(test[DatabaseHelper.Picture3]);
                        // Retrieve image
                        if (test[DatabaseHelper.Picture4] != null)
                            temp.Picture4 = (Byte[])(test[DatabaseHelper.Picture4]);
                    }
                }
            }
            return temp;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FilmModel model, SelectListItem mylist)
        {
            // Add new User to Database
            ModelState.Clear();

            model.listCate = getListCate();

            model.listTech = mergeList(model.listTech);
            // Kiem tra du lieu
            ValidateData(model);

            if (ModelState.IsValid)
            {
                try
                {
                    // Get img from post method
                    #region image
                    byte[] img = new byte[0];
                    HttpPostedFileBase file = Request.Files["file"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img = new byte[file.ContentLength];
                        file.InputStream.Read(img, 0, file.ContentLength);
                    }

                    byte[] img1 = new byte[0];
                    file = Request.Files["file1"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img1 = new byte[file.ContentLength];
                        file.InputStream.Read(img1, 0, file.ContentLength);
                    }

                    byte[] img2 = new byte[0];
                    file = Request.Files["file2"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img2 = new byte[file.ContentLength];
                        file.InputStream.Read(img2, 0, file.ContentLength);
                    }

                    byte[] img3 = new byte[0];
                    file = Request.Files["file3"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img3 = new byte[file.ContentLength];
                        file.InputStream.Read(img3, 0, file.ContentLength);
                    }

                    byte[] img4 = new byte[0];
                    file = Request.Files["file4"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img4 = new byte[file.ContentLength];
                        file.InputStream.Read(img4, 0, file.ContentLength);
                    }
                    #endregion

                    // Get current User
                    int UserID = WebMatrix.WebData.WebSecurity.CurrentUserId;

                    //Insert into database
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"Insert into Film values (@UserID,@CateID,@Name,@Actor,@Dire,@Desc,@Pic,@StaDate,@EndDate,@TechID,@Com,@Pic1,@Pic2,@Pic3,@Pic4,@long)";

                        // Xet xem film duoc add vao voi nhung cong nghe nao (2D,3D)
                        foreach (var item in model.listTech)
                        {
                            if (item.Selected)
                            {
                                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                                {
                                    // Add value
                                    cmd.Parameters.AddWithValue("@UserID", UserID);
                                    cmd.Parameters.AddWithValue("@CateID", Int32.Parse(model.CategoryID));

                                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = model.Name + " (" + item.Text + ")" ;

                                    if (String.IsNullOrEmpty(model.Actor))
                                    {
                                        cmd.Parameters.AddWithValue("@Actor", DBNull.Value);
                                    }
                                    else
                                        cmd.Parameters.Add("@Actor", SqlDbType.NVarChar).Value = model.Actor;

                                    if (String.IsNullOrEmpty(model.Director))
                                    {
                                        cmd.Parameters.AddWithValue("@Dire", DBNull.Value);
                                    }
                                    else
                                        cmd.Parameters.Add("@Dire", SqlDbType.NVarChar).Value = model.Director;

                                    if (String.IsNullOrEmpty(model.Description))
                                    {
                                        cmd.Parameters.AddWithValue("@Desc", DBNull.Value);
                                    }
                                    else
                                        cmd.Parameters.Add("@Desc", SqlDbType.NVarChar).Value = model.Description;

                                   
                                    cmd.Parameters.Add("@Pic", SqlDbType.VarBinary).Value = img;
                                    cmd.Parameters.Add("@StaDate", SqlDbType.DateTime).Value = model.StartDate;
                                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = model.EndDate.AddDays(1);

                                    cmd.Parameters.Add("@TechID", SqlDbType.Int).Value = Int32.Parse(item.Value);
                                    cmd.Parameters.Add("@Com", SqlDbType.NVarChar).Value = model.Company;
                                    cmd.Parameters.Add("@long", SqlDbType.Int).Value = model.filmLong;

                                    cmd.Parameters.Add("@Pic1", SqlDbType.VarBinary).Value = img1;
                                    cmd.Parameters.Add("@Pic2", SqlDbType.VarBinary).Value = img2;
                                    cmd.Parameters.Add("@Pic3", SqlDbType.VarBinary).Value = img3;
                                    cmd.Parameters.Add("@Pic4", SqlDbType.VarBinary).Value = img4;
                                    // Exec
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        conn.Close();
                        conn.Dispose();
                    }
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }

            return RedirectToAction("List");
        }

        private List<SelectListItem> mergeList(List<SelectListItem> list)
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from TechOfFilm";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int TechID = (int)reader[DatabaseHelper.TechID];
                            string Name = reader[DatabaseHelper.Feature].ToString();
                            mylist.Add(new SelectListItem() { Value = TechID.ToString(), Text = Name , Selected= list[TechID-1].Selected});
                        }
                    }
                }
            return mylist;
        }

        private void ValidateData(FilmModel model)
        {
            if (model.StartDate > model.EndDate)
                ModelState.AddModelError("", "Start Date cannot later than End Date");

            bool flag = false;
            foreach (var item in model.listTech)
            {
                if (item.Selected)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                ModelState.AddModelError("", "At least one tech of Film must be chose");

        }

        public ActionResult Delete(int FilmID)
        {
            var model = getFilmByID(FilmID);
            if (model.StartDate < DateTime.UtcNow.AddHours(7) && DateTime.UtcNow.AddHours(7) < model.EndDate.AddDays(1))
                return View();
            
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();

                // Delete from Schedule
                string sqlSelect = @"Delete from Schedule Where FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", FilmID.ToString());
                    cmd.ExecuteNonQuery();

                }

                // Delete from Film
                sqlSelect = @"Delete from Film Where FilmID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", FilmID.ToString());
                    cmd.ExecuteNonQuery();

                }
                conn.Close();
                conn.Dispose();
            }
            return RedirectToAction("List");
        }
    }
}
