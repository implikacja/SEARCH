using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searcher.Models {

    public class Article {
        public string title;
        public string description;
        public DateTime date;
        public string dateStr;
        public double[] TF;
        public double[] TF_IDF;
        public double[] TFYear;
        public double[] TFTitle;
        public double relevance;
        public Author[] authors;
        public List<Author> authors2;
        public int Id;
        private static int IdNumerator = 0;
        public bool relevant { get; set; }
        public bool irrelevant { get; set; }

        public Article() {
            this.Id = 0;
            this.title = "";
            this.description = "";
            this.date = DateTime.MinValue;
            this.authors2 = new List<Author>();
            this.dateStr = "00000000";
        }

        public Article(bool newId) {
            this.Id = 0;
            if (newId) {
                this.Id = IdNumerator++;
            };
            this.title = "";
            this.description = "";
            this.date = DateTime.MinValue;
            this.authors2 = new List<Author>();
            this.dateStr = "00000000";
        }

        public string getAuthorsList() {
            string auth = "";

            foreach (Author a in authors)
                auth += a.firstName + " " + a.lastName + Environment.NewLine + Environment.NewLine;

            return auth;
        }

        public string getTFList() {
            return String.Join(", ", TF);
        }

        public string getTF_IDFList() {
            return String.Join(", ", TF_IDF);
        }

        public void createDate() {
            Int32 y, m, d;
            y = 0;
            m = 0;
            d = 0;
            string s = dateStr;

            System.Diagnostics.Debug.WriteLine("s" + s);
            if (s.Contains('.'))
            {
                d = int.Parse(s.Substring(0, s.IndexOf('.')));
                s = s.Substring(s.IndexOf('.') + 1);
                m = int.Parse(s.Substring(0, s.IndexOf('.')));
                s = s.Substring(s.IndexOf('.') + 1);
                y = int.Parse(s.Substring(0, 4));
            }
            else if (s.Contains('/'))
            {
                m = int.Parse(s.Substring(0, s.IndexOf('/')));
                s = s.Substring(s.IndexOf('/') + 1);
                d = int.Parse(s.Substring(0, s.IndexOf('/')));
                s = s.Substring(s.IndexOf('/') + 1);
                y = int.Parse(s.Substring(0, 4));
            }

            date = new DateTime(y, m, d);
        }


        public void CountRelevance(string MarkValue, double[] queryTF, double[] queryTF_IDF) {
            int queryCount = Dictionary.dictionary.Count;
            double r = 0f;
            double qLength = 0f;
            double aLength = 0f;
            if (MarkValue == "TF")
            {
                for (int i = 0; i < queryCount; i++)
                {
                    r += (queryTF[i] * TF[i]);
                    qLength += (queryTF[i] * queryTF[i]);
                    aLength += (TF[i] * TF[i]);
                }

            }
            else if (MarkValue == "IDF")
            {
                for (int i = 0; i < queryCount; i++)
                {
                    if (queryTF_IDF[i] != 0)
                        System.Diagnostics.Debug.WriteLine("TF_IDF: " + TF_IDF[i]);
                    r += (queryTF_IDF[i] * TF_IDF[i]);
                    qLength += (queryTF_IDF[i] * queryTF_IDF[i]);
                    aLength += (TF_IDF[i] * TF_IDF[i]);
                }

            }
            else if (MarkValue == "YEAR")//QueryTF == QueryYear
            {
                for (int i = 0; i < queryCount; i++)
                {
                    r += (queryTF[i] * TF[i]);
                    qLength += (queryTF[i] * queryTF[i]);
                    aLength += (TFYear[i] * TFYear[i]);
                }
            }
            else if (MarkValue == "TITLE")//QueryTF == QueryYear
            {
                for (int i = 0; i < queryCount; i++)
                {
                    r += (queryTF[i] * TF[i]);
                    qLength += (queryTF[i] * queryTF[i]);
                    aLength += (TFTitle[i] * TFTitle[i]);
                }
            }
            if (qLength == 0f)
            {
                relevance = 0f;
                return;
            }
            qLength = Math.Sqrt(qLength);
            aLength = Math.Sqrt(aLength);
            relevance = r / (qLength * aLength);

            System.Diagnostics.Debug.WriteLine("alength: " + aLength + ", qlength: " + qLength + ", r: " + r);


        }
    }
}
