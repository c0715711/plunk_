using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestPunkApi.Models
{
    public class BeerRating
    {
        public int BeerId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }

    }
}