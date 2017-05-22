using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace searcher.Models {
    class UseXML {
        public double[] queryTF;
        public double[] queryTF_IDF;
        public double[] queryTFYear;

        public List<Article> doList(List<string> searchWords) {
            string query = "";
            foreach (String word in searchWords) {
                query += word + " ";
            }

            List<Article> articles = SearchIndex.FindArticles(query);

            int numOfWords = Dictionary.dictionary.Count;
            int[] searchWordsCount = new int[numOfWords];
            double[] IDF = new double[numOfWords];

            for (int i = 0; i < numOfWords; i++) {
                searchWordsCount[i] = 0;
                IDF[i] = 0;
            }

            foreach (Article art in articles) {
                countTermsFrequencies(art, searchWords);
                countYearFrequencies(art);
                System.Diagnostics.Debug.WriteLine("doc TF: " + String.Join(", ", art.TF));


                for (int i = 0; i < numOfWords; i++) {
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

            article.TF = t.countTermsFrequencies(Dictionary.dictionary);
        }

        private void countYearFrequencies(Article article, int year, double alfa) {
            article.TFYear = article.TF;
            if (article.date.Year < year) {
                for (int i = 0; i < article.TFYear.Length; i++) {
                    article.TFYear[i] *= alfa;
                }
            }
        }

        public void countTermsFrequenciesQuery(List<string> searchWords) {
            int numOfWords = Dictionary.dictionary.Count;
            queryTF = new double[numOfWords];
            queryTFYear = new double[numOfWords];

            for (int i = 0; i < numOfWords; i++)
                queryTF[i] = 0f;

            foreach (var d in Dictionary.dictionary) {
                foreach (string s in searchWords) {
                    if (s.Equals(d.Key))
                        queryTF[d.Value] += 1f;
                }
            }

            double maxTF = queryTF.Max();

            if (maxTF != 0)
                for (int i = 0; i < numOfWords; i++)
                    queryTF[i] /= maxTF; // tokens.Count;

            for (int i = 0; i < queryTF.Length; i++)
            {
                queryTFYear[i] = queryTF[i];
            }
        }

        private void countYearFrequencies(Article article)
        {
            article.TFYear = new double[article.TF.Length];
            for (int i = 0; i < article.TF.Length; i++)
            {
                article.TFYear[i] = article.TF[i];
            }
            int sub = DateTime.Now.Year- article.date.Year+1;
                for (int i = 0; i < article.TFYear.Length; i++)
                {
                    article.TFYear[i] *= ((double)sub/20);
                }
            
        }
        public void countTF_IDF(List<string> searchWords) {
            int numOfWords = Dictionary.dictionary.Count;
            queryTF_IDF = new double[numOfWords];

            for (int i = 0; i < numOfWords; i++)
                queryTF_IDF[i] = 0.0;


        }

        public void rocchio(List<Article> articles, string MarkValue, double alfa, double beta, double gamma) {
            if (MarkValue == "TF") {
                for (int i = 0; i < queryTF.Length; i++) {
                    queryTF[i] *= alfa;
                }
                int rel = 0;
                double[] valueRel;
                int irrel = 0;
                double[] valueIrrel;
                foreach (var article in articles) {
                    if (article.relevant) {
                        rel++;
                        valueRel = new double[article.TF.Length];
                        for (int i = 0; i < valueRel.Length; i++) {
                            valueRel[i] = 0f;
                        }

                        for (int i = 0; i < article.TF.Length; i++) {
                            valueRel[i] += article.TF[i];
                        }

                        for (int i = 0; i < queryTF.Length; i++) {
                            queryTF[i] += (beta * (valueRel[i] / rel));
                        }
                    }
                    if (article.irrelevant) {
                        irrel++;
                        valueIrrel = new double[article.TF.Length];
                        for (int i = 0; i < valueIrrel.Length; i++) {
                            valueIrrel[i] = 0f;
                        }

                        for (int i = 0; i < article.TF.Length; i++) {
                            valueIrrel[i] += article.TF[i];
                        }

                        for (int i = 0; i < queryTF.Length; i++) {
                            queryTF[i] -= (gamma * (valueIrrel[i] / irrel));
                            if (queryTF[i] < 0f) {
                                queryTF[i] = 0f;
                            }
                        }
                    }
                }
            } else if (MarkValue == "IDF") {
                for (int i = 0; i < queryTF_IDF.Length; i++) {
                    queryTF_IDF[i] *= alfa;
                }
                int rel = 0;
                double[] valueRel;
                int irrel = 0;
                double[] valueIrrel;
                foreach (var article in articles) {
                    if (article.relevant) {
                        rel++;
                        valueRel = new double[article.TF_IDF.Length];
                        for (int i = 0; i < valueRel.Length; i++) {
                            valueRel[i] = 0f;
                        }

                        for (int i = 0; i < article.TF_IDF.Length; i++) {
                            valueRel[i] += article.TF_IDF[i];
                        }

                        for (int i = 0; i < queryTF_IDF.Length; i++) {
                            queryTF_IDF[i] += (beta * (valueRel[i] / rel));
                        }
                    }
                    if (article.irrelevant) {
                        irrel++;
                        valueIrrel = new double[article.TF_IDF.Length];
                        for (int i = 0; i < valueIrrel.Length; i++) {
                            valueIrrel[i] = 0f;
                        }

                        for (int i = 0; i < article.TF_IDF.Length; i++) {
                            valueIrrel[i] += article.TF_IDF[i];
                        }

                        for (int i = 0; i < queryTF_IDF.Length; i++) {
                            queryTF_IDF[i] -= (gamma * (valueIrrel[i] / irrel));
                            if (queryTF_IDF[i] < 0f) {
                                queryTF_IDF[i] = 0f;
                            }
                        }
                    }
                }
            } else if (MarkValue == "YEAR") {
                for (int i = 0; i < queryTFYear.Length; i++) {
                    queryTFYear[i] *= alfa;
                }
                int rel = 0;
                double[] valueRel;
                int irrel = 0;
                double[] valueIrrel;
                foreach (var article in articles) {
                    if (article.relevant) {
                        rel++;
                        valueRel = new double[article.TFYear.Length];
                        for (int i = 0; i < valueRel.Length; i++) {
                            valueRel[i] = 0f;
                        }

                        for (int i = 0; i < article.TFYear.Length; i++) {
                            valueRel[i] += article.TFYear[i];
                        }

                        for (int i = 0; i < queryTFYear.Length; i++) {
                            queryTFYear[i] += (beta * (valueRel[i] / rel));
                        }
                    }
                    if (article.irrelevant) {
                        irrel++;
                        valueIrrel = new double[article.TFYear.Length];
                        for (int i = 0; i < valueIrrel.Length; i++) {
                            valueIrrel[i] = 0f;
                        }

                        for (int i = 0; i < article.TFYear.Length; i++) {
                            valueIrrel[i] += article.TFYear[i];
                        }

                        for (int i = 0; i < queryTFYear.Length; i++) {
                            queryTFYear[i] -= (gamma * (valueIrrel[i] / irrel));
                            if (queryTFYear[i] < 0f) {
                                queryTFYear[i] = 0f;
                            }
                        }
                    }
                }
            }
        }


        public void weightedTerms(string MarkValue, int[] weights, List<string> searchWords) {
            int sum = weights.Sum();
            if (MarkValue == "TF") {
                foreach (var d in Dictionary.dictionary) {
                    for (int j = 0; j < searchWords.Count(); j++) {
                        if (d.Key.Equals(searchWords[j])) {
                            queryTF[d.Value] *= (weights[j] / (double)sum);
                        }
                    }

                }
            } else if (MarkValue == "IDF") {
                foreach (var d in Dictionary.dictionary) {
                    for (int j = 0; j < searchWords.Count(); j++) {
                        if (d.Key.Equals(searchWords[j])) {
                            queryTF_IDF[d.Value] *= (weights[j] / (double)sum);
                        }
                    }

                }
            } else if (MarkValue == "YEAR") {
                foreach (var d in Dictionary.dictionary) {
                    for (int j = 0; j < searchWords.Count(); j++) {
                        if (d.Key.Equals(searchWords[j])) {
                            queryTFYear[d.Value] *= (weights[j] / (double)sum);
                        }
                    }

                }
            }
        }


    }
}