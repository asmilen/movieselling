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
    public class ManageFilmController : Controller
    {
        //
        // GET: /ManageFilm/
        public ActionResult List()
        {
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
                        try
                        {
                            int FilmID = (int)test[DatabaseColumnName.FilmID];
                            int UserID = (int)test[DatabaseColumnName.UserID];
                            string CateID = test[DatabaseColumnName.CategoryID].ToString();
                            string Name = test[DatabaseColumnName.Name].ToString();
                            string TechID = test[DatabaseColumnName.TechID].ToString();

                            // Vi Actor,director,Description co the null nen phai kiem tra
                            string Actor = "";
                            if (test[DatabaseColumnName.Actor] != null)
                                Actor = test[DatabaseColumnName.Actor].ToString();

                            string Direc = "";
                            if (test[DatabaseColumnName.Director] != null)
                                Direc = test[DatabaseColumnName.Director].ToString();

                            string Desc = "";
                            if (test[DatabaseColumnName.Description] != null)
                                Desc = test[DatabaseColumnName.Description].ToString();

                            DateTime StartDate = (DateTime)test[DatabaseColumnName.StartDate];
                            DateTime EndDate = (DateTime)test[DatabaseColumnName.EndDate];

                            byte[] data = new byte[0];
                            templist.Add(new FilmModel(FilmID,UserID,CateID,Name,Actor, Direc,Desc,data,StartDate,EndDate,TechID));
                        }
                        catch (Exception ex)
                        {
                            // Co loi trong luc load database, bo qua user co loi
                        }
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
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from CategoryFilm";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int FilmID = (int)reader[DatabaseColumnName.CategoryID];
                            string Name = reader[DatabaseColumnName.Name].ToString();
                            mylist.Add(new SelectListItem() { Value = FilmID.ToString(), Text = Name });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
            }
            mylist[0].Selected = true;
            return mylist;
        }

        private List<SelectListItem> getListTech()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from TechOfFilm";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int TechID = (int)reader[DatabaseColumnName.TechID];
                            string Name = reader[DatabaseColumnName.Feature].ToString();
                            mylist.Add(new SelectListItem() { Value = TechID.ToString(), Text = Name });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
            }
            return mylist;
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
                    // Get current User
                    int UserID = WebMatrix.WebData.WebSecurity.CurrentUserId;

                    //Insert into database
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"Insert into Film values (@UserID,@CateID,@Name,@Actor,@Dire,@Desc,@Pic,@StaDate,@EndDate,@TechID)";

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

                                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = model.Name;

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

                                    // Get img from post method
                                    byte[] img = new byte[0];
                                    HttpPostedFileBase file = Request.Files["file"];
                                    if (file != null && file.ContentLength > 0)
                                    {
                                        img = new byte[file.ContentLength];
                                        file.InputStream.Read(img, 0, file.ContentLength);
                                    }
                                    cmd.Parameters.Add("@Pic", SqlDbType.VarBinary).Value = img;
                                    cmd.Parameters.Add("@StaDate", SqlDbType.DateTime).Value = model.StartDate;
                                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = model.EndDate;

                                    cmd.Parameters.Add("@TechID", SqlDbType.Int).Value = Int32.Parse(item.Value);
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
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                {
                    string sqlSelect = @"select * from TechOfFilm";
                    using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int TechID = (int)reader[DatabaseColumnName.TechID];
                            string Name = reader[DatabaseColumnName.Feature].ToString();
                            mylist.Add(new SelectListItem() { Value = TechID.ToString(), Text = Name , Selected= list[TechID-1].Selected});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Film nao bi loi thi bo qua
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
