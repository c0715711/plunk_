using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestPunkApi.Models
{
    public class DbJson
    {
        public List<BeerRating> userRatings { get; set; }
        public List<Beer> searchbeers { get; set; }
    }
}