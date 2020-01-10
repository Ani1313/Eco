using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace EcoFootprint.Models
{
    [ComplexType]
    public class Score
    {
        public double CropLand { get; set; }
        public double GrazingLand { get; set; }
        public double FishingGrounds { get; set; }
        public double BuiltUpLand { get; set; }
        public double CarbonFootprint { get; set; }
        public double Forest { get; set; }
        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        public double Total => CropLand + GrazingLand + FishingGrounds + BuiltUpLand + CarbonFootprint + Forest;


        public static Score operator +(Score s1, Score s2)
        {
            return new Score
            {
                CropLand = s1.CropLand + s2.CropLand,
                GrazingLand = s1.GrazingLand + s2.GrazingLand,
                FishingGrounds = s1.FishingGrounds + s2.FishingGrounds,
                BuiltUpLand = s1.BuiltUpLand + s2.BuiltUpLand,
                CarbonFootprint = s1.CarbonFootprint + s2.CarbonFootprint,
                Forest = s1.Forest + s2.Forest,
            };
        }


        public static Score operator -(Score s1, Score s2)
        {
            return new Score
            {
                CropLand = s1.CropLand - s2.CropLand,
                GrazingLand = s1.GrazingLand - s2.GrazingLand,
                FishingGrounds = s1.FishingGrounds - s2.FishingGrounds,
                BuiltUpLand = s1.BuiltUpLand - s2.BuiltUpLand,
                CarbonFootprint = s1.CarbonFootprint - s2.CarbonFootprint,
                Forest = s1.Forest - s2.Forest,
            };
        }
        public static Score operator *(Score s, double num)
        {
            return new Score
            {
                CropLand = s.CropLand * num,
                GrazingLand = s.GrazingLand * num,
                FishingGrounds = s.FishingGrounds * num,
                BuiltUpLand = s.BuiltUpLand * num,
                CarbonFootprint = s.CarbonFootprint * num,
                Forest = s.Forest * num
            };
        }
           public static Score operator *( double num,Score s)
        {
            return new Score
            {
                CropLand = s.CropLand * num,
                GrazingLand = s.GrazingLand * num,
                FishingGrounds = s.FishingGrounds * num,
                BuiltUpLand = s.BuiltUpLand * num,
                CarbonFootprint = s.CarbonFootprint * num,
                Forest = s.Forest * num
            };
        }

    }
}