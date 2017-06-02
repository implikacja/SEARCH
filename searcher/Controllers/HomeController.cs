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
                ViewBag.Message = result;
                ViewBag.Terms = t.getTokens();

                UseXML x = new UseXML();
                List<Article> articles = x.doList(t.getTokens());
                string MarkValue = Request.Form["Marks"].ToString();
                ViewBag.Mark = MarkValue;
                string SupportValue = Request.Form["Support"].ToString();
                ViewBag.Support = SupportValue;

                x.countTermsFrequenciesQuery(t.getTokens());

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
                    case "Weighted terms": {
                            string[] w = new string[t.getTokens().Count()];
                            w = Request.Form.GetValues("intList");
                            int[] weights = w.Select(int.Parse).ToArray();
                            ViewBag.W = weights;
                            if (weights.Length == t.getTokens().Count())
                                x.weightedTerms(MarkValue, weights, t.getTokens());
                            break;
                        }
                    default: break;
                }

                foreach (var a in articles) {
                    a.CountRelevance(MarkValue, x.queryTF, x.queryTF_IDF);
                }

                var tokens = t.getTokens();
                if (SupportValue == "Weighted terms") {
                    // statystyka

                    foreach (string tok in tokens) {
                        if (Dictionary.dictionary.ContainsKey(tok)) {
                            foreach (Article a in articles) {
                                a.TFTermposition.Add(tok, 0);
                            }

                            int dicTermPos = Dictionary.dictionary[tok];

                            articles.Sort((b, a) => a.TF[dicTermPos].CompareTo(b.TF[dicTermPos]));
                            for (int i = 0; i < articles.Count; i++) {
                                articles[i].TFTermposition[tok] = i;
                            }
                        }
                    }
                }

                articles.Sort((a, b) => a.relevance.CompareTo(b.relevance));
                articles.Reverse();

                if (SupportValue == "Weighted terms") {
                    var change = new Dictionary<string, double>();
                    double measure;
                    double val;
                    foreach (string tok in tokens) {
                        if (Dictionary.dictionary.ContainsKey(tok)) {
                            measure = 0.0;
                            for (int i = 0; i < articles.Count; i++) {
                                val = (double)(articles[i].TFTermposition[tok] - i + 1);
                                val = (val > 0 ? val : -val);
                                if (val != 0.0)
                                    measure += (1.0 / val);
                                System.Diagnostics.Debug.Write(tok + ": " + measure);
                            }
                            measure /= articles.Count;
                            measure = Math.Round(measure, 3);
                            change.Add(tok, measure);
                        } else {
                            change.Add(tok, 0.0);
                        }
                    }

                    ViewBag.Change = change;
                }

                foreach (Article a in articles)
                    a.relevance = Math.Round(a.relevance, 3);

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