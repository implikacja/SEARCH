using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searcher.Models
{
    enum Marks
    {
        TF,
        IDF
    }
    public class Article
    {
        public string title;
        public string description;
        public DateTime date;
        public double[] TF;
        public double[] TF_IDF;
        public float relevance;
        public Author[] authors;

        public Article() 
        {

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

        public void CountRelevance(string MarkValue)
        {

        }
    }
}


