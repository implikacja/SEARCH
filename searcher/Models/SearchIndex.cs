using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace searcher.Models {
    public class SearchIndex {
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

        private static void _addToLuceneIndex(Article article, IndexWriter writer) {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", article.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", article.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", article.title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", article.description, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Date", article.date.ToString(), Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }

        private static Article _mapLuceneDocumentToData(Document doc) {
            return new Article {
                Id = Convert.ToInt32(doc.Get("Id")),
                title = doc.Get("Name"),
                description = doc.Get("Description"),
                dateStr = doc.Get("Date")
            };
        }

        private static IEnumerable<Article> _mapLuceneToDataList(IEnumerable<Document> hits) {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }

        private static IEnumerable<Article> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher) {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

    }
}