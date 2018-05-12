using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Pdf_project.Database;

namespace Pdf_project.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            //Check if user is logged, if is open dashboard
            if (Convert.ToInt32(Session["isLoggedIn"]) == 1 && Session["userZip"].ToString().Trim() == Request["zip"].ToString().Trim())
            {
                ViewBag.Zip = Request["zip"].ToString().Trim();
                ViewBag.Email = Request["email"].ToString().Trim();

                //Get data from db for current user 
                hopeCRMEntitiesSecond db = new hopeCRMEntitiesSecond();
                string zip = Request["zip"].ToString().Trim();

                Customer CurrentUser = db.Customers.Where(t => t.zip.Trim() == zip).First();

              
                //Adding user data to viewbag
                if (CurrentUser.name1!=null)
                {
                    ViewBag.AgName1 = CurrentUser.name1.ToString().Trim();
                }

                if (CurrentUser.name2 != null)
                {
                    ViewBag.AgName2 = CurrentUser.name2.ToString().Trim();
                }

                if (CurrentUser.street != null)
                {
                    ViewBag.Street = CurrentUser.street.ToString().Trim() + " " + CurrentUser.streetno.ToString().Trim();
                }

                if (CurrentUser.zip != null)
                {
                    ViewBag.Zip = CurrentUser.zip.ToString().Trim();
                }

                if (CurrentUser.city != null)
                {
                    ViewBag.City = CurrentUser.city.ToString().Trim();
                }

                if (CurrentUser.countryid != null)
                {
                    ViewBag.Country = CurrentUser.countryid.ToString().Trim();
                }

                if (CurrentUser.email != null)
                {
                    ViewBag.AgContact = CurrentUser.email.ToString().Trim();
                }


                return View();
            }

            //If not open login page
            else
            {
                return RedirectToAction("index", "home");
            }
            
        }
    }
}