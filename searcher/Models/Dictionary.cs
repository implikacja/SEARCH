using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace searcher.Models
{
    public static class Dictionary
    {
        public static Dictionary<String, int> dictionary;
        private static int no;

        public static void buildDictionary2(List <Article> articles) {
            no = 0;

            if (dictionary == null) {
                dictionary = new Dictionary<string, int>();
            } else {
                dictionary.Clear();
            }

            TokenizeStopStem t;

            foreach (var article in articles) {
                t = new TokenizeStopStem(article.description);
                t.tokenize();
                addTokens(t);
                t = new TokenizeStopStem(article.title);
                t.tokenize();
                addTokens(t);
            }
        }

        private static void addTokens(TokenizeStopStem t) {
            List<String> tokens = t.getTokens();

            foreach (String token in tokens) {
                if (!dictionary.ContainsKey(token)) {
                    dictionary.Add(token, no++);
                }
            }
        }

        public static void saveDictionary()
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(dictionary);
            var path = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\dictionary.json");
            File.WriteAllText(path, json);
        }
    }
}
