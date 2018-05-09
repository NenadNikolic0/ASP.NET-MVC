using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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