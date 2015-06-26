using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Manage.Models;
using System.Data.SqlClient;

namespace Manage.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManageUserController : Controller
    {
        //
        // GET: /ManageUser/

        public ActionResult List()
        {
            List<User> mylist = getListUser();
            return View(mylist);
        }

        private List<User> getListUser()
        {
            List<User> templist = new List<User>();
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
                        int UserID = (int)test[DatabaseColumnName.UserID];
                        int RoleID = (int)test[DatabaseColumnName.RoleID];
                        string Name = test[DatabaseColumnName.FullName].ToString();
                        DateTime DateOfBirth = (DateTime)test[DatabaseColumnName.DateOfBirth];
                        string Address = test[DatabaseColumnName.Address].ToString();
                        string username = test[DatabaseColumnName.username].ToString();
                        // Retrieve image
                        //Byte[] data = new Byte[0];
                        //data = (Byte[])(test[DatabaseColumnName.Picture]);
                        //MemoryStream mem = new MemoryStream(data);
                        templist.Add(new User(UserID, RoleID, Name, DateOfBirth, null, Address, username));
                    }
                }
            }
            return templist;
        }

        public ActionResult Add()
        {
            ListRole mylist = new ListRole();
            ViewBag.dropdownlist = mylist.dropdownlistRole;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(User model, SelectListItem mylist)
        {
            model.RoleID = Int32.Parse(model.RoleName);
            WebSecurity.CreateUserAndAccount(model.username, model.password, new { RoleID = model.RoleID, Name = model.Name });
            switch(model.RoleID){
                case 1:
                    Roles.AddUserToRole(model.username, "Administrator");
                    break;
                case 2:
                    Roles.AddUserToRole(model.username,"Manager");
                    break;
                case 3:
                    Roles.AddUserToRole(model.username, "Staff");
                    break;
            }
            return View(model);
        }
    }
}
