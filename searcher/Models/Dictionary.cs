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
        public static void buildDictionary(List<Article> articles)
        {
            int nr = 0;
            foreach (var article in articles)
            {
                if (dictionary == null)
                    dictionary = new Dictionary<string, int>();
                TokenizeStopStem t = new TokenizeStopStem(article.description);
                t.tokenize();
                List<String> tokens = t.getTokens();
                foreach (String token in tokens)
                {
                    if (!dictionary.ContainsKey(token))
                    {
                        dictionary.Add(token, nr);
                        nr++;
                    }
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

        public static void loadDictionary()
        {
            var serializer = new JavaScriptSerializer();
            //var deserializedResult = serializer.Deserialize<Dictionary<String,int>>(serializedResult);
        }
    }
}
