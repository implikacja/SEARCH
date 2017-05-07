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
        XDocument xdoc;
        String input;

        public UseXML(String input)
        {
            //var fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\pubmed_result.xml");
            var fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\test.xml");
            this.xdoc = XDocument.Load(fileName);
            this.input = input;
        }

        public List<Article> doList()
        {
            List<Article> articles =
                (
                    from a in xdoc.Element("PubmedArticleSet").Elements("PubmedArticle")
                    select new Article 
                    {
                        title = (string)a.Element("MedlineCitation").Element("Article").Element("ArticleTitle"),
                        description = (string)a.Element("MedlineCitation").Element("Article").Element("Abstract")
                    }

                ).ToList();


            return articles;
        }
    }
}
