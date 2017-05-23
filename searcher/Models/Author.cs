using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace searcher.Models {
    public class Author {

        public string firstName;
        public string lastName;

        public Author() {

        }

        public Author(string firstName, string lastName) {
            this.firstName = firstName;
            this.lastName = lastName;
        }
    }
}