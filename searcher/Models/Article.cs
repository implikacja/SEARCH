﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searcher.Models
{
    enum Marks
    {
        TF,
        IDF
    }
    public class Article
    {
        public string title;
        public string description;
        public DateTime date;
        public string dateStr;
        public double[] TF;
        public double[] TF_IDF;
        public double relevance;
        public Author[] authors;
        public int Id;
        private static int IdNumerator = 0;

        public Article() 
        {
            this.Id = IdNumerator++;
        }

        public string getAuthorsList() {
            string auth = "";

            foreach (Author a in authors)
                auth += a.firstName + " " + a.lastName + Environment.NewLine + Environment.NewLine;

            return auth;
        }

        public string getTFList() {
            return String.Join(", ", TF);
        }

        public string getTF_IDFList() {
            return String.Join(", ", TF_IDF);
        }

        public void CountRelevance(string MarkValue, double[] queryTF)
        {
            if(MarkValue == "TF")
            {
                int queryCount = Dictionary.dictionary.Count;
                double r = 0f;
                double qLength = 0f;
                double aLength = 0f;
                for (int i = 0; i < queryCount; i++)
                {
                    r += (queryTF[i] * TF[i]);
                    qLength += (queryTF[i] * queryTF[i]);
                    aLength += (TF[i] * TF[i]);
                }
                if(qLength == 0f)
                {
                    relevance = 0f;
                    return;
                }
                qLength = Math.Sqrt(qLength);
                aLength = Math.Sqrt(aLength);
                relevance = r / (qLength * aLength);
            }

        }
    }
}


