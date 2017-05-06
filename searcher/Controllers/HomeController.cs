using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace searcher.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}