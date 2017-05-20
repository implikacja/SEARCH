﻿using System;
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
        public DateTime date;
        public string dateStr;
        public double[] TF;
        public double[] TF_IDF;
        public float relevance;
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
    }
}
