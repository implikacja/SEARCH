using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searcher.Models
{
    public class Article
    {
        public String title;
        public String keywords;
        public float TF;
        public float relevance;
        public Article(String title, String keywords)
        {
            this.title = title;
            this.keywords = keywords;
        }
    }
}
