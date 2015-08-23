using Manage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manage.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManageRoomController : Controller
    {
        //
        // GET: /ManageRoom/

        public ActionResult List()
        {
            List<RoomModel> mylist = getListRoom();
            return View(mylist);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RoomModel model)
        {
            // Add new User to Database
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"Insert into Room values (@name,@row,@col)";

                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            // Add value
                            cmd.Parameters.AddWithValue("@row", model.NumberOfRow);
                            cmd.Parameters.AddWithValue("@col", model.NumberOfColumn);
                            cmd.Parameters.AddWithValue("@name", model.RoomName);
                            // Exec
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                        conn.Dispose();
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


        private List<RoomModel> getListRoom()
        {
            List<RoomModel> mylist = new List<RoomModel>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * from Room ";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    conn.Open();
                    SqlDataReader test = cmd.ExecuteReader();
                    while (test.Read())
                    {
                        try
                        {
                            int RoomId = (int)test[DatabaseHelper.RoomID];
                            string Name = test[DatabaseHelper.Name].ToString();
                            string NumberOfColumn = test[DatabaseHelper.NumberOfColumn].ToString();
                            string NumberOfRow = test[DatabaseHelper.NumberOfRow].ToString();
                            mylist.Add(new RoomModel(RoomId, NumberOfRow, NumberOfColumn, Name));
                        }
                        catch (Exception ex)
                        {
                            // Co loi trong luc load database, bo qua user co loi
                        }
                    }
                }
            }
            return mylist;
        }

        public ActionResult Edit(int RoomID)
        {
            RoomModel model = getRoomByID(RoomID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomModel model)
        {
            // Edit exist User to Database
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    //Update vao database
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"update Room set Name = @name, NumberOfColumn = @col, NumberOfRow = @row where RoomID = @ID ";
                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", model.RoomName);
                            cmd.Parameters.AddWithValue("@col", model.NumberOfColumn);
                            cmd.Parameters.AddWithValue("@row", model.NumberOfRow);
                            cmd.Parameters.AddWithValue("@ID", model.RoomID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }
            }
            return RedirectToAction("List");
        }

        private RoomModel getRoomByID(int RoomID)
        {
             RoomModel model = new RoomModel();
             using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
             {
                 string sqlSelect = @"select * from Room Where RoomID = @ID";
                 using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                 {
                     conn.Open();
                     cmd.Parameters.AddWithValue("@ID", RoomID.ToString());
                     SqlDataReader test = cmd.ExecuteReader();
                     while (test.Read())
                     {
                         model.RoomID = (int)(test[DatabaseHelper.RoomID]);
                         model.RoomName = (test[DatabaseHelper.Name]).ToString();
                         model.NumberOfColumn = (test[DatabaseHelper.NumberOfColumn]).ToString();
                         model.NumberOfRow = (test[DatabaseHelper.NumberOfRow]).ToString();
                     }
                 }
             }
             return model;
        }

        public ActionResult Delete(int RoomID)
        {
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();

                string sqlSelect = @"Delete from Room Where RoomID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", RoomID.ToString());
                    cmd.ExecuteNonQuery();

                }
                conn.Close();
                conn.Dispose();
            }
            return RedirectToAction("List");
        }
    }
}
