using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searcher.Models
{
    class TokenizeStopStem
    {
        public String input;
        public TokenizeStopStem(String input)
        {
        this.input = input;
        }
        public String tokenization()
        {
            List<String> result = new List<String>();

            TokenStream tokenStream = new StandardTokenizer(Lucene.Net.Util.Version.LUCENE_30, new StringReader(this.input));
            tokenStream = new StopFilter(false,tokenStream, StandardAnalyzer.STOP_WORDS_SET);
            tokenStream = new PorterStemFilter(tokenStream);


            StringBuilder sb = new StringBuilder();

            var termAttr = tokenStream.GetAttribute<Lucene.Net.Analysis.Tokenattributes.ITermAttribute>();

            while (tokenStream.IncrementToken())
            {
                sb.Append(termAttr.Term);
                sb.Append(" ");
            }

            return sb.ToString();

        }
    }
}
