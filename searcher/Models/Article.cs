using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searcher.Models
{

    public class Article
    {
        public string title;
        public string description;
        public DateTime date;
        public float[] termsFrequencies;
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

        public string getTermsFrequenciesList() {
            return String.Join(", ", termsFrequencies);
        }
    }
}
