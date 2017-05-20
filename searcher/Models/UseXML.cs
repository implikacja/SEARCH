using System;
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
        String input;

        public UseXML(String input)
        {
            this.input = input;
        }

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

        private void countTermsFrequencies(Article article, List<string> searchWords) {
            TokenizeStopStem t = new TokenizeStopStem(article.description);
            t.tokenize();

            article.TF = t.countTermsFrequencies(searchWords);
        }

    }
}
