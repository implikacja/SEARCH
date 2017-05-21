using searcher.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace searcher.Controllers {
    public class HomeController : Controller {

        public ActionResult Index(string command, string fileToIndex, string searchString, int[] relevantList, int[] irrelevantList) {
            if (command == "Search" && !String.IsNullOrEmpty(searchString)) {

                TokenizeStopStem t = new TokenizeStopStem(searchString);
                t.tokenize();
                string result = t.getTokensList();
                //String result = t.tokenization();
                ViewBag.Message = result;

                UseXML x = new UseXML();
                List<Article> articles = x.doList(t.getTokens());
                string MarkValue = Request.Form["Marks"].ToString();
                ViewBag.Mark = MarkValue;
                string SupportValue = Request.Form["Support"].ToString();
                ViewBag.Support = SupportValue;
                switch (MarkValue) {
                    case "TF": {
                            x.countTermsFrequenciesQuery(t.getTokens());
                            break;
                        }
                    case "IDF": {

                            break;
                        }
                    default: break;
                }

                switch (SupportValue) {
                    case "Relevance feedback": {

                            foreach (var article in articles) {
                                article.relevant = false;
                                article.irrelevant = false;
                                if (relevantList != null) {
                                    if (relevantList.Contains(article.Id))
                                        article.relevant = true;
                                }
                                if (irrelevantList != null) {
                                    if (irrelevantList.Contains(article.Id))
                                        article.irrelevant = true;
                                }

                                if (article.relevant == true && article.irrelevant == true) {
                                    article.relevant = false;
                                    article.irrelevant = false;
                                }

                                x.rocchio(articles, MarkValue, 1, 1, 0.5);
                            }
                            break;
                        }
                    default: break;
                }

                foreach (var a in articles) {
                    a.CountRelevance(MarkValue, x.queryTF, x.queryTF_IDF);
                }
                articles.Sort((a, b) => a.relevance.CompareTo(b.relevance));
                articles.Reverse();
                return View(articles);
            }

            if (command == "Create index") {
                Data.newLoad(fileToIndex);
            }

            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}