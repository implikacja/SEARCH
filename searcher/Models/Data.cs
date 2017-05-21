using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace searcher.Models {
    public static class Data {

        private static XDocument xdoc;
        public static List<Article> articles;

        public static void newLoad(string fileName) {
            string _dataDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\" + fileName + ".xml");
            int max = 500;
            int i = 1;
            var articlesToIndex = new List<Article>();
            Article article = new Article();

            System.Diagnostics.Debug.WriteLine("Articles indexed from file: " + fileName);

            XmlReader xmlReader = XmlReader.Create(_dataDir);
            while (xmlReader.Read()) {

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "PubmedArticle") {
                    article = new Article();
                } else
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ArticleTitle") {
                    xmlReader.Read();
                    article.title = xmlReader.Value;
                } else
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Abstract") {
                    while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Abstract")) {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "AbstractText") {
                            xmlReader.Read();
                            article.description += xmlReader.Value + " ";
                        }
                        xmlReader.Read();
                    }
                } else
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "DateCreated") {
                    string y = "0000";
                    string m = "01";
                    string d = "01";

                    while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "DateCreated")) {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Year") {
                            xmlReader.Read();
                            y = xmlReader.Value;
                        }
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Month") {
                            xmlReader.Read();
                            m = xmlReader.Value;
                        }
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Day") {
                            xmlReader.Read();
                            d = xmlReader.Value;
                        }
                        xmlReader.Read();
                    }

                    article.dateStr = y + m + d;
                    article.date = new DateTime((Int32)int.Parse(y), (Int32)int.Parse(m), (Int32)int.Parse(d));
                } else
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "AuthorList") {
                    Author author;

                    while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "AuthorList")) {

                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Author") {

                            author = new Author();

                            while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Author")) {

                                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "LastName") {
                                    xmlReader.Read();
                                    author.lastName = xmlReader.Value;
                                }
                                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ForeName") {
                                    xmlReader.Read();
                                    author.firstName = xmlReader.Value;
                                }
                                xmlReader.Read();
                            }

                            article.authors2.Add(author);
                        }
                        xmlReader.Read();
                    }
                } else
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "PubmedArticle") {
                    articlesToIndex.Add(article);

                    if (++i > max) {
                        i = 1;
                        SearchIndex.AddUpdateLuceneIndex(articlesToIndex);
                        articlesToIndex.Clear();
                    }
                }
            }

            SearchIndex.AddUpdateLuceneIndex(articlesToIndex);
            articlesToIndex.Clear();
        }

        public static void load() {
            string _dataDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\test.xml");
            xdoc = XDocument.Load(_dataDir);

            articles =
             (
                from a in xdoc.Element("PubmedArticleSet").Elements("PubmedArticle")
                select new Article {
                    title = (string)a.Element("MedlineCitation").Element("Article").tryToGetXMLValue("ArticleTitle"),
                    description = (string)a.Element("MedlineCitation").Element("Article").tryToGetXMLValue("Abstract")
                    ,
                    date = new DateTime((Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Year"),
                                        (Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Month"),
                                        (Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Day")),
                    authors =
                    (
                        from auth in a.Element("MedlineCitation").Element("Article").Element("AuthorList").Elements("Author")
                        select new Author {
                            firstName = (string)auth.Element("ForeName"),
                            lastName = (string)auth.Element("LastName")
                        }
                    ).ToArray()
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