using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Pdf_project.Database;



namespace Pdf_project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            return View();
            
        }
 
    }
}