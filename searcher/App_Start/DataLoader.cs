using searcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace searcher.App_Start {
    public class DataLoader {

        public static void Init() {
            Data.load();
        }

    }
}