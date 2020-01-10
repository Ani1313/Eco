using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoFootprint.Models
{
    public class FinalResults
    {

        public DateTime EarthOvershootDay { get; set; }
        public double EarthsNeed { get; set; }

        public Score TotalScore { get;  set; }
        public double TotalFood { get;  set; }
        public double TotalHousing { get;  set; }
        public double TotalGoods { get;  set; }
        public double TotalSeruices { get;  set; }
        public double TotalMobility { get;  set; }

    }
}