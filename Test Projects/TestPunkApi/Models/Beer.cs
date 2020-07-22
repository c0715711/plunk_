using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestPunkApi.Models
{
    public class Beer
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public List<BeerRating> beerRatings { get; set; }
    }
}