using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Pdf_project.Database;
using Pdf_project.Models;



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

            //Checking if customer exists or not and filling Result variable with corresponding value
            if (customer.Count!=0)
            {
                Result = "True";
                Session["isLoggedIn"] = 1;
            }
            else
            {
                Result = "False";
            }

            //Returning Json as result 
            return Json(Result);
        }

 
    }
}