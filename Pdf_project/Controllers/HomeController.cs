using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Pdf_project.Database;
using Pdf_project.Models;
using System.IO;
using System.Text;
using System.Globalization;

namespace Pdf_project.Controllers
{
    public class HomeController : Controller
    {
        // Default Home controller action that will return login form 
        public ActionResult Index()
        {
            //Set Session isLoggedIn property to 0
            Session["isLoggedIn"] = 0;
            return View();
            
        }

        // Controller action that will receive data from login form, check in database if user with that password exists (zip as username and serial no as password) and
        // and will return true or false as Result 
        [HttpPost]
        public ActionResult Login(Login model)
        {
            //Declaring variable Result that will hold true or false, depends on db search result
            String Result;

            //Declaring and instantiating hopeCRMEntities object (db instance)
            hopeCRMEntities db = new hopeCRMEntities();

            //Declaring List of customers that will contain certain customer or null
            List<Customer> customer = db.Customers.Where(t => t.zip == model.username.ToString().Trim() && t.serialno == model.password.ToString().Trim()).ToList();

            //Define empty User object 
            User user = new User();

            //Checking if customer exists or not and filling Result variable with corresponding value
            if (customer.Count!=0)
            {
                Result = "True";
                Session["isLoggedIn"] = 1;
                user.Result = Result;
                user.UserZip = customer[0].zip.ToString().Trim();
                user.UserEmail = customer[0].email.ToString().Trim();


                //Writting into log file (user zip, email, date and time)
                using (StreamWriter writer = new StreamWriter(Server.MapPath("~/Log/log.txt"), true))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(user.UserZip + " " + user.UserEmail + " " + DateTime.Now.ToString("dd.MM.yyyy hh:mm tt",CultureInfo.InvariantCulture));
                    writer.WriteLine(sb.ToString());
                }


            }
            else
            {
                Result = "False";
                user.Result = Result;              
            }


            //Returning user as result in Json format
            return Json(user);
        }

 
    }
}