using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pdf_project.Controllers
{
    public class PageNotFoundController : Controller
    {
        // GET: NonexistentRoute
        public ActionResult Index()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}