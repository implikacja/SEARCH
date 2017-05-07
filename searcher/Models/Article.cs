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
        public string keywords;
        public float TF;
        public float relevance;

        public Article() 
        {

        }

        public Article(string title, string keywords)
        {
            this.title = title;
            this.keywords = keywords;
        }
    }
}
