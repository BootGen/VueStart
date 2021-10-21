using System;
using System.Collections.Generic;

namespace StartVue
{
    public class Visitor {

        public int Id { get; set; }
        public string Token { get; set; }
        public string CountryCode { get; set; }
        public List<Visit> Visits { get; set; }
    }
}