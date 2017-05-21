using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace searcher.Models {
    public static class Data {

        //private static string _dataDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\test.xml");
        private static XDocument xdoc;
        public static List<Article> articles;

        public static void load() {
            string _dataDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\test.xml");
            xdoc = XDocument.Load(_dataDir);

            articles =
             (
                from a in xdoc.Element("PubmedArticleSet").Elements("PubmedArticle")
                select new Article {
                    title = (string)a.Element("MedlineCitation").Element("Article").tryToGetXMLValue("ArticleTitle"),
                    description = (string)a.Element("MedlineCitation").Element("Article").tryToGetXMLValue("Abstract")
                    //,
                    //date = new DateTime((Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Year"),
                    //                    (Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Month"),
                    //                    (Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Day")),
                    //authors =
                    //(
                    //    from auth in a.Element("MedlineCitation").Element("Article").Element("AuthorList").Elements("Author")
                    //    select new Author {
                    //        firstName = (string)auth.Element("ForeName"),
                    //        lastName = (string)auth.Element("LastName")
                    //    }
                    //).ToArray()
                }
            ).ToList();
        }

        private static string tryToGetXMLValue(this XElement parentEl, string elementName, string defaultValue = "") {

            if (parentEl != null) {
                var foundEl = parentEl.Element(elementName);
                if (foundEl != null) {
                    return foundEl.Value;
                }
            }

            return defaultValue;
        }

    }
}