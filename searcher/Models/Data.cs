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
            string _dataPath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\" + fileName + ".xml");
            if (File.Exists(_dataPath)) {
                int max = 500;
                int i = 1;
                var articlesToIndex = new List<Article>();
                Article article = new Article(false);

                System.Diagnostics.Debug.WriteLine("Articles indexed from file: " + fileName);

                XmlReader xmlReader = XmlReader.Create(_dataPath);
                while (xmlReader.Read()) {

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "PubmedArticle") {
                        article = new Article(true);
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

                                article.authors.Add(author);
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
        }
    }
}