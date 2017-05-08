﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace searcher.Models
{
    class UseXML
    {
        XDocument xdoc;
        String input;

        public UseXML(String input)
        {
            //var fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\pubmed_result.xml");
            var fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\test.xml");
            this.xdoc = XDocument.Load(fileName);
            this.input = input;
        }

        public List<Article> doList(List<string> searchWords)
        {
            List<Article> articles =
                (
                    from a in xdoc.Element("PubmedArticleSet").Elements("PubmedArticle")
                    select new Article 
                    {
                        title = (string)a.Element("MedlineCitation").Element("Article").Element("ArticleTitle"),
                        description = (string)a.Element("MedlineCitation").Element("Article").Element("Abstract"),
                        date = new DateTime((Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Year"),
                                            (Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Month"),
                                            (Int32)a.Element("MedlineCitation").Element("DateCreated").Element("Day")),
                        authors =
                        (
                            from auth in a.Element("MedlineCitation").Element("Article").Element("AuthorList").Elements("Author")
                            select new Author 
                            {
                                firstName = (string)auth.Element("ForeName"),
                                lastName = (string)auth.Element("LastName")
                            }
                        ).ToArray()
                    }

                ).ToList();

            int[] searchWordsCount = new int[searchWords.Count];
            double[] IDF = new double[searchWords.Count];

            for (int i = 0; i < searchWords.Count; i++) {
                searchWordsCount[i] = 0;
                IDF[i] = 0;
            }

            foreach (Article art in articles) {
                countTermsFrequencies(art, searchWords);
                System.Diagnostics.Debug.WriteLine("doc TF: " + String.Join(", ", art.TF));

                for (int i = 0; i < searchWords.Count; i++) {
                    if (art.TF[i] > 0)
                        searchWordsCount[i]++;
                }
            }

            for (int i = 0; i < searchWords.Count; i++) {
                if (searchWordsCount[i] >= 1)
                    IDF[i] = Math.Log10((double)articles.Count / searchWordsCount[i]);
            }

            System.Diagnostics.Debug.WriteLine("--------------------------");
            System.Diagnostics.Debug.WriteLine("word in how many texts: " + String.Join(", ", searchWordsCount.ToArray()));
            System.Diagnostics.Debug.WriteLine("IDF: " + String.Join(", ", IDF.ToArray()));

            foreach (Article art in articles) {
                art.TF_IDF = new double[searchWords.Count];

                for (int i = 0; i < searchWords.Count; i++) {
                    art.TF_IDF[i] = art.TF[i] * IDF[i];
                }
            }

            return articles;
        }

        private void countTermsFrequencies(Article article, List<string> searchWords) {
            TokenizeStopStem t = new TokenizeStopStem(article.description);
            t.tokenize();

            article.TF = t.countTermsFrequencies(searchWords);
        }

    }
}
