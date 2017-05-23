using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace searcher.Models {
    public class SearchIndex {

        public static readonly int MAX_SEARCH = 100;
        public static readonly String ID = "Id";
        public static readonly String TITLE = "Title";
        public static readonly String DESCRIPTION = "Description";
        public static readonly String DATE = "Date";

        private static string _luceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory {
            get {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        public static void AddUpdateLuceneIndex(IEnumerable<Article> articles) {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED)) {
                // add data to lucene search index (replaces older entry if any)
                foreach (var article in articles) _addToLuceneIndex(article, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public static void AddUpdateLuceneIndex(Article article) {
            AddUpdateLuceneIndex(new List<Article> { article });
        }

        public static List<Article> FindArticles(string searchQuery) {
            System.Diagnostics.Debug.WriteLine("Searching for articles for query: " + searchQuery);

            IndexSearcher indexSearcher = new IndexSearcher(_directory);
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, DESCRIPTION, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            Query query = parser.Parse(searchQuery);


            TopDocs top = indexSearcher.Search(query, MAX_SEARCH);
            var articles = new List<Article>();

            ScoreDoc[] results = top.ScoreDocs;

            System.Diagnostics.Debug.WriteLine(top.TotalHits);

            int luceneID;
            Article article;
            foreach (ScoreDoc item in results) {
                luceneID = item.Doc;
                Document doc = indexSearcher.Doc(luceneID);

                article = new Article(false);
                String value;
                value = doc.GetField(ID).ToString();
                value = cutDocString(value);
                article.Id = int.Parse(value);
                value = doc.GetField(DESCRIPTION).ToString();
                value = cutDocString(value);
                article.description = value;
                value = doc.GetField(TITLE).ToString();
                value = cutDocString(value);
                article.title = value;
                value = doc.GetField(DATE).ToString();
                value = cutDocString(value);
                article.dateStr = value;
                article.createDate();
                articles.Add(article);
            }

            indexSearcher.Dispose();
            return articles;
        }

        private static String cutDocString(String s) {
            s = s.Substring(s.IndexOf(':') + 1);
            s = s.Substring(0, s.IndexOf('>'));
            return s;
        }

        private static void _addToLuceneIndex(Article article, IndexWriter writer) {
            // remove older index entry
            var searchQuery = new TermQuery(new Term(ID, article.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field(ID, article.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(TITLE, article.title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(DESCRIPTION, article.description, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(DATE, article.date.ToString(), Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }
    }
}