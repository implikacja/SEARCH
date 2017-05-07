using searcher.Models;
using System;
using System.Collections;
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
                TokenizeStopStem t = new TokenizeStopStem(searchString);
                String result = t.tokenization();
                ViewBag.Message = result;
                UseXML x = new UseXML(result);
                List<Article> articles = x.doList();
                return View(articles);
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