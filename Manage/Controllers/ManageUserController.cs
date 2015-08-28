using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Manage.Models;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Globalization;

namespace Manage.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManageUserController : Controller
    {
        //
        // GET: /ManageUser/

        public ActionResult List()
        {
            List<ViewUser> mylist = getListUser();
            return View(mylist);
        }

        public ActionResult Delete(int UserID)
        {
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                conn.Open();

                // Check khong xoa duoc chinh minh
                if (WebSecurity.CurrentUserId == UserID)
                {
                    ModelState.AddModelError("", "Cannot Delete Yourself");
                    return RedirectToAction("List");
                }


                // Delete from User
                string sqlSelect = @"Delete from webpages_OAuthMembership Where UserId = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", UserID.ToString());
                    cmd.ExecuteNonQuery();

                }

                // Delete from User
                sqlSelect = @"Delete from webpages_UsersInRoles Where UserId = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", UserID.ToString());
                    cmd.ExecuteNonQuery();

                }

                // Delete from User
                sqlSelect = @"Delete from webpages_Membership Where UserId = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", UserID.ToString());
                    cmd.ExecuteNonQuery();
                }

                // Delete from User
                 sqlSelect = @"Delete from Users Where UserId = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", UserID.ToString());
                    cmd.ExecuteNonQuery();

                }
                conn.Close();
                conn.Dispose();
            }
            return RedirectToAction("List");
        }

        public ActionResult ViewDetail(int UserID)
        {
            ViewUser model = getUserByID(UserID);
            return View(model);
        }

        private ViewUser getUserByID(int UserID)
        {
            ViewUser model = new ViewUser();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * from Users Where UserID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID", UserID.ToString());
                    SqlDataReader test = cmd.ExecuteReader();
                    while (test.Read())
                    {
                        model.Name = test[DatabaseHelper.FullName].ToString();
                        model.username = test[DatabaseHelper.username].ToString();

                        // Vi Address co the null nen phai kiem tra
                        if (test[DatabaseHelper.Address] != null)
                            model.address = test[DatabaseHelper.Address].ToString();

                        if (test[DatabaseHelper.DateOfBirth] != null)
                            model.DateOfBirth = DateTime.ParseExact(test[DatabaseHelper.DateOfBirth].ToString(), DatabaseHelper.DateFormat, CultureInfo.InvariantCulture);

                        // Retrieve image
                        if (test[DatabaseHelper.Picture] != null)
                            model.picture = (Byte[])(test[DatabaseHelper.Picture]);

                        model.RoleName = Roles.GetRolesForUser(model.username)[0];
                    }
                }
                conn.Close();
                conn.Dispose();
            }
            return model;
        }
        private List<ViewUser> getListUser()
        {
            List<ViewUser> templist = new List<ViewUser>();
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string sqlSelect = @"select * 
                        from Users ";
                using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                {
                    conn.Open();
                    SqlDataReader test = cmd.ExecuteReader();
                    while (test.Read())
                    {
                        int UserID = (int)test[DatabaseHelper.UserID];
                        string Name = test[DatabaseHelper.FullName].ToString();
                        string username = test[DatabaseHelper.username].ToString();

                        // Vi Address co the null nen phai kiem tra
                        string Address = "";
                        if (test[DatabaseHelper.Address] != null)
                            Address = test[DatabaseHelper.Address].ToString();

                        DateTime DateOfBirth = DateTime.ParseExact(test[DatabaseHelper.DateOfBirth].ToString(), DatabaseHelper.DateFormat, CultureInfo.InvariantCulture);

                        templist.Add(new ViewUser(UserID, Name, DateOfBirth, null, Address, username));
                    }
                }
            }
            return templist;
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(User model, SelectListItem mylist)
        {
            // Add new User to Database
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    byte[] img = new byte[0];
                    HttpPostedFileBase file = Request.Files["file"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img = new byte[file.ContentLength];
                        file.InputStream.Read(img, 0, file.ContentLength);
                    }

                    String currentDateOfBirth = model.DateOfBirth.ToString(DatabaseHelper.DateFormat);
                    // Create new user
                    WebSecurity.CreateUserAndAccount(model.username, model.password, new { Name = model.Name, Picture = img, DateOfBirth = currentDateOfBirth, Address = model.address });

                    // Gan Role cho user moi
                    int roleID = Int32.Parse(model.RoleName);
                    Roles.AddUserToRole(model.username, DatabaseHelper.listRoles.ElementAt(roleID - 1).Text);
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

        public ActionResult Edit(int UserID)
        {
            ViewUser model = getUserByID(UserID);

            // Chon lai gia tri mac dinh cho list role
            DatabaseHelper.setNewSelectedRole(model.RoleName);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewUser model,SelectListItem item)
        {
            // Edit exist User to Database
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    // lay file
                    byte[] img = null;
                    HttpPostedFileBase file = Request.Files["file"];
                    if (file != null && file.ContentLength > 0)
                    {
                        img = new byte[file.ContentLength];
                        file.InputStream.Read(img, 0, file.ContentLength);
                    }

                   // xoa role hien tai
                    Roles.RemoveUserFromRole(model.username, Roles.GetRolesForUser(model.username)[0]);

                    // Gan Role moi
                    int roleID = Int32.Parse(model.RoleName);
                    Roles.AddUserToRole(model.username, DatabaseHelper.listRoles.ElementAt(roleID - 1).Text);

                    //Update vao database
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string sqlSelect = @"update users set Name = @name, DateOfBirth = @date, Address = @add where username = @username ";
                        using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", model.Name);
                            cmd.Parameters.AddWithValue("@date", model.DateOfBirth.ToString(DatabaseHelper.DateFormat));
                            cmd.Parameters.AddWithValue("@add", model.address);
                            cmd.Parameters.AddWithValue("@username", model.username);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                        conn.Open();
                        if (img != null) 
                        {
                            sqlSelect = @"update users set Picture = @img where username = @username";
                            using (SqlCommand cmd = new SqlCommand(sqlSelect, conn))
                            {
                                cmd.Parameters.AddWithValue("@username", model.username);
                                cmd.Parameters.Add("@img", SqlDbType.VarBinary).Value = img;
                                cmd.ExecuteNonQuery();
                            }
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
    }
}
