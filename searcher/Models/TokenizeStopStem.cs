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
        List<string> tokens = new List<string>();


        public TokenizeStopStem(String input)
        {
            this.input = input;
        }

        public void tokenize() {
            TokenStream tokenStream = new StandardTokenizer(Lucene.Net.Util.Version.LUCENE_30, new StringReader(this.input));
            tokenStream = new StopFilter(false, tokenStream, StandardAnalyzer.STOP_WORDS_SET);
            tokenStream = new PorterStemFilter(tokenStream);

            var termAttr = tokenStream.GetAttribute<Lucene.Net.Analysis.Tokenattributes.ITermAttribute>();

            while (tokenStream.IncrementToken())
                tokens.Add(termAttr.Term);
        }

        public string getTokensList() {
            return String.Join(", ", tokens.ToArray());
        }

        public List<string> getTokens() {
            return tokens;
        }

        public double[] countTermsFrequencies(List<string> searchWords) {
            int numOfSearchWords = searchWords.Count;
            double[] tf = new double[numOfSearchWords];

            for (int i = 0; i < numOfSearchWords; i++)
                tf[i] = 0f;

            foreach (string token in tokens) {
                for (int i = 0; i < numOfSearchWords; i++) {
                    if (token.Equals(searchWords[i]))
                        tf[i] += 1f;
                }
            }

            double maxTF = tf.Max();

            if (maxTF != 0)
                for (int i = 0; i < numOfSearchWords; i++)
                    tf[i] /= maxTF; // tokens.Count;

            return tf;
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
