﻿using System;
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
            Session["userZip"] = 0;
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

            using (DSGVOEntities1 db = new DSGVOEntities1())
            {

                db.Database.Connection.Open();
                //Declaring List of customers that will contain certain customer or null
                List<kunden> customer = db.kundens.Where(t => t.plz == model.password.ToString().Trim() && t.seriennr.Substring(15).Trim() == model.username.ToString().Trim()).ToList();

                //Define empty User object 
                UserInfo user = new UserInfo();

                //Checking if customer exists or not and filling user object with corresponding value
                if (customer.Count != 0)
                {
                    Result = "True";
                    Session["isLoggedIn"] = 1;
                    Session["userZip"] = customer[0].plz.ToString().Trim();
                    Session["serialNo"] = customer[0].seriennr.Substring(15).Trim();
                    user.Result = Result;
                    user.UserZip = customer[0].plz.ToString().Trim();
                    user.UserEmail = customer[0].email.ToString().Trim();
                    user.UserSerialNo = customer[0].seriennr.Substring(15).Trim();

                    //Writting into log file (user zip, email, date and time)
                    using (StreamWriter writer = new StreamWriter(Server.MapPath("~/Log/log.txt"), true))
                    {
                        //Declaring and instatiating object of String Builder class, that will append current row 
                        StringBuilder sb = new StringBuilder();
                        sb.Append(user.UserZip + " " + user.UserEmail + " " + DateTime.Now.ToString("dd.MM.yyyy hh:mm tt", CultureInfo.InvariantCulture));

                        //Write row with login data into txt file
                        writer.WriteLine(sb.ToString());
                    }


                }
                else
                {
                    //If user is not found set false as result
                    Result = "False";
                    user.Result = Result;
                }


                //Returning user as result in Json format
                return Json(user);

            }

        }


    }
}