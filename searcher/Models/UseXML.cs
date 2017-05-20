using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace searcher.Models
{
    class UseXML
    {
        public double[] queryTF;


        public List<Article> doList(List<string> searchWords)
        {
            List<Article> articles = Data.articles;

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

        private void countTermsFrequencies(Article article, List<string> searchWords)
        {
            TokenizeStopStem t = new TokenizeStopStem(article.description);
            t.tokenize();

            article.TF = t.countTermsFrequencies(Dictionary.dictionary);
        }

        public void countTermsFrequenciesQuery(List<string> searchWords)
        {
            int numOfWords = Dictionary.dictionary.Count;
            queryTF = new double[numOfWords];

            for (int i = 0; i < numOfWords; i++)
                queryTF[i] = 0f;

            foreach (var d in Dictionary.dictionary)
            {
                foreach (string s in searchWords)
                {
                    if (s.Equals(d.Key))
                        queryTF[d.Value] += 1f;
                }
            }

            double maxTF = queryTF.Max();

            if (maxTF != 0)
                for (int i = 0; i < numOfWords; i++)
                    queryTF[i] /= maxTF; // tokens.Count;
        }



    }
}
