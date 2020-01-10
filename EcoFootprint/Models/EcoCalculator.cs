using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcoFootprint.DAL;
namespace EcoFootprint.Models
{

    /*
         public enum QuestionCategory
    {
        None,
        Food,
        Housing,
        Goods,
        Seruices,
        Mobility
    }
            public double CropLand { get; set; }
        public double GrazingLand { get; set; }
        public double FishingGrounds { get; set; }
        public double BuiltUpLand { get; set; }
        public double CarbonFootprint { get; set; }
        public double Forest { get; set; }

 
      */
    public static class EcoCalculator
    {
        public static Score TotalScore { get; private set; } = new Score();
        public static double TotalFood { get; private set; }
        public static double TotalHousing { get; private set; }
        public static double TotalGoods { get; private set; }
        public static double TotalSeruices { get; private set; }
        public static double TotalMobility { get; private set; }
        public static double Total => TotalFood + TotalHousing + TotalGoods + TotalSeruices + TotalMobility;


        public static void Calculate(Question question, Answer answer)
        {

            switch (question.Category)
            {
                case QuestionCategory.None:
                    throw new Exception("Question Category is none");

                case QuestionCategory.Food:
                    TotalFood += answer.Score.Total;
                    break;
                case QuestionCategory.Housing:
                    TotalHousing += answer.Score.Total;
                    break;
                case QuestionCategory.Goods:
                    TotalGoods += answer.Score.Total;
                    break;
                case QuestionCategory.Seruices:
                    TotalSeruices += answer.Score.Total;
                    break;
                case QuestionCategory.Mobility:
                    TotalMobility += answer.Score.Total;
                    break;
            }

            TotalScore += answer.Score;
        }

        public static void Calculate(Question question, int slidebarValue)
        {
            Slidebar slidebar = question.Slidebar;
            Score score = slidebar.MinScore + ((slidebar.MaxScore - slidebar.MinScore) * (slidebarValue / 100.0));
            switch (question.Category)
            {
                case QuestionCategory.None:
                    throw new Exception("Question Category is none");

                case QuestionCategory.Food:
                    TotalFood += score.Total;
                    break;
                case QuestionCategory.Housing:
                    TotalHousing += score.Total;
                    break;
                case QuestionCategory.Goods:
                    TotalGoods += score.Total;
                    break;
                case QuestionCategory.Seruices:
                    TotalSeruices += score.Total;
                    break;
                case QuestionCategory.Mobility:
                    TotalMobility += score.Total;
                    break;
            }

            TotalScore += score;
        }

        public static FinalResults GetFinalResults()
        {
            EcoFootprintContext db = new EcoFootprintContext();
            List<Question> questions = db.Questions.ToList();
            double minTotal = questions.Sum(q =>q.Answers==null||q.Answers.Count==0? 0:q.Answers.Min(a => a.Score.Total));


            return new FinalResults
            {
                TotalScore = TotalScore,
                TotalFood = TotalFood,
                TotalHousing = TotalHousing,
                TotalGoods = TotalGoods,
                TotalSeruices = TotalSeruices,
                TotalMobility = TotalMobility,
                //todo:earth overshoot day
                EarthOvershootDay = DateTime.Now,
                EarthsNeed = Total / minTotal
            };
        }

    }
}