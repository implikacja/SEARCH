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
        public double[] queryTFTitle;

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
                countTitleFrequencies(art, searchWords);
                System.Diagnostics.Debug.WriteLine("doc TF: " + String.Join(", ", art.TF));


                for (int i = 0; i < numOfWords; i++) {
                    if (art.TF[i] > 0)
                        searchWordsCount[i]++;
                }
            }

            double ratio;
            for (int i = 0; i < numOfWords; i++) {
                if (searchWordsCount[i] >= 1) {
                    ratio = (double)articles.Count / searchWordsCount[i];
                    if (ratio == 1) {
                        IDF[i] = 0.02;
                    } else {
                        IDF[i] = Math.Log10(ratio);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("--------------------------");
            System.Diagnostics.Debug.WriteLine("word in how many texts: " + String.Join(", ", searchWordsCount.ToArray()));
            System.Diagnostics.Debug.WriteLine("IDF: " + String.Join(", ", IDF.ToArray()));

            foreach (Article art in articles) {
                art.TF_IDF = new double[numOfWords];

                for (int i = 0; i < numOfWords; i++) {
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
            queryTF_IDF = new double[numOfWords];
            queryTFYear = new double[numOfWords];
            queryTFTitle = new double[numOfWords];

            for (int i = 0; i < numOfWords; i++) {
                queryTF[i] = 0f;
                queryTF_IDF[i] = 0f;
            }

            foreach (string s in searchWords) {
                if (Dictionary.dictionary.ContainsKey(s))
                    queryTF[Dictionary.dictionary[s]] += 1f;
            }

            double maxTF = queryTF.Max();

            if (maxTF != 0)
                for (int i = 0; i < numOfWords; i++)
                    queryTF[i] /= maxTF; // tokens.Count;

            for (int i = 0; i < queryTF.Length; i++)
            {
                if (queryTF[i] == 0) {
                    queryTF_IDF[i] = 0;
                } else {
                    queryTF_IDF[i] = 1;
                }

                queryTFYear[i] = queryTF[i];
                queryTFTitle[i] = queryTF[i];
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

        private void countTitleFrequencies(Article article, List<string> searchWords)
        {
            article.TFTitle = new double[article.TF.Length];
            for (int i = 0; i < article.TF.Length; i++)
            {
                article.TFTitle[i] = article.TF[i];
            }
            int count = 1;
            TokenizeStopStem title = new TokenizeStopStem(article.title);
            title.tokenize();
            List<string> t = title.getTokens();
            foreach(var item in t)
            {
                foreach(string s in searchWords)
                {
                    if(item.Equals(s))
                    {
                        count++;
                    }
                }
            }
            for (int i = 0; i < article.TFTitle.Length; i++)
            {
                article.TFTitle[i] *= (1/(double)count);
            }

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
                    queryTF[i] *= alfa;
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
                            queryTF[i] += (beta * (valueRel[i] / rel));
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
                            queryTF[i] -= (gamma * (valueIrrel[i] / irrel));
                            if (queryTF[i] < 0f) {
                                queryTF[i] = 0f;
                            }
                        }
                    }
                }
            }
            else if (MarkValue == "IDF")
            {
                for (int i = 0; i < queryTF_IDF.Length; i++)
                {
                    queryTF_IDF[i] *= alfa;
                }
                int rel = 0;
                double[] valueRel;
                int irrel = 0;
                double[] valueIrrel;
                foreach (var article in articles)
                {
                    if (article.relevant)
                    {
                        rel++;
                        valueRel = new double[article.TF_IDF.Length];
                        for (int i = 0; i < valueRel.Length; i++)
                        {
                            valueRel[i] = 0f;
                        }

                        for (int i = 0; i < article.TF_IDF.Length; i++)
                        {
                            valueRel[i] += article.TF_IDF[i];
                        }

                        for (int i = 0; i < queryTF_IDF.Length; i++)
                        {
                            queryTF_IDF[i] += (beta * (valueRel[i] / rel));
                        }
                    }
                    if (article.irrelevant)
                    {
                        irrel++;
                        valueIrrel = new double[article.TF_IDF.Length];
                        for (int i = 0; i < valueIrrel.Length; i++)
                        {
                            valueIrrel[i] = 0f;
                        }

                        for (int i = 0; i < article.TF_IDF.Length; i++)
                        {
                            valueIrrel[i] += article.TF_IDF[i];
                        }

                        for (int i = 0; i < queryTF_IDF.Length; i++)
                        {
                            queryTF_IDF[i] -= (gamma * (valueIrrel[i] / irrel));
                            if (queryTF_IDF[i] < 0f)
                            {
                                queryTF_IDF[i] = 0f;
                            }
                        }
                    }
                }
            }
            else if (MarkValue == "TITLE")
            {
                for (int i = 0; i < queryTFYear.Length; i++)
                {
                    queryTF[i] *= alfa;
                }
                int rel = 0;
                double[] valueRel;
                int irrel = 0;
                double[] valueIrrel;
                foreach (var article in articles)
                {
                    if (article.relevant)
                    {
                        rel++;
                        valueRel = new double[article.TFTitle.Length];
                        for (int i = 0; i < valueRel.Length; i++)
                        {
                            valueRel[i] = 0f;
                        }

                        for (int i = 0; i < article.TFTitle.Length; i++)
                        {
                            valueRel[i] += article.TFTitle[i];
                        }

                        for (int i = 0; i < queryTFTitle.Length; i++)
                        {
                            queryTF[i] += (beta * (valueRel[i] / rel));
                        }
                    }
                    if (article.irrelevant)
                    {
                        irrel++;
                        valueIrrel = new double[article.TFTitle.Length];
                        for (int i = 0; i < valueIrrel.Length; i++)
                        {
                            valueIrrel[i] = 0f;
                        }

                        for (int i = 0; i < article.TFTitle.Length; i++)
                        {
                            valueIrrel[i] += article.TFTitle[i];
                        }

                        for (int i = 0; i < queryTFTitle.Length; i++)
                        {
                            queryTF[i] -= (gamma * (valueIrrel[i] / irrel));
                            if (queryTF[i] < 0f)
                            {
                                queryTF[i] = 0f;
                            }
                        }
                    }
                }
            }
        }


        public void weightedTerms(string MarkValue, int[] weights, List<string> searchWords) {
            int sum = weights.Sum();
            if (MarkValue == "TF")
            {
                foreach (var d in Dictionary.dictionary)
                {
                    for (int j = 0; j < searchWords.Count(); j++)
                    {
                        if (d.Key.Equals(searchWords[j]))
                        {
                            queryTF[d.Value] *= (weights[j] / (double)sum);
                        }
                    }
                }
            }
            else if (MarkValue == "IDF")
            {
                foreach (var d in Dictionary.dictionary)
                {
                    for (int j = 0; j < searchWords.Count(); j++)
                    {
                        if (d.Key.Equals(searchWords[j]))
                        {
                            queryTF_IDF[d.Value] *= (weights[j] / (double)sum);
                        }
                    }
                }
            }
            else if (MarkValue == "YEAR")
            {
                foreach (var d in Dictionary.dictionary)
                {
                    for (int j = 0; j < searchWords.Count(); j++)
                    {
                        if (d.Key.Equals(searchWords[j]))
                        {
                            queryTF[d.Value] *= (weights[j] / (double)sum);
                        }
                    }
                }
            } else if (MarkValue == "TITLE") {
                foreach (var d in Dictionary.dictionary) {
                    for (int j = 0; j < searchWords.Count(); j++) {
                        if (d.Key.Equals(searchWords[j])) {
                            queryTF[d.Value] *= (weights[j] / (double)sum);
                        }
                    }

                }
            } 
        }


    }
}