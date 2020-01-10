using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EcoFootprint.Models;
using EcoFootprint.Abstract;
using System.Data;
using System.Data.SqlClient;
using EcoFootprint.DAL;
namespace EcoFootprint.Controllers
{
    public class HomeController : Controller
    {
        EcoFootprintContext db = new EcoFootprintContext();
        static bool IsAM;
        public ActionResult Start()
        {
            return View();
        }

        public ActionResult SelectLanguage()
        {
            return View();
        }
        public ActionResult SetLanguage(bool? isAM)
        {
            if (isAM == null)
                return HttpNotFound();

            IsAM = (bool)isAM;
            return RedirectToAction("Index");
        }

        public ActionResult Index(int? questionId)
        {
            ViewBag.IsAM = IsAM;
            if (questionId == null)
            {
                int id = db.Questions.Min(x => x.ID);
                return View(db.Questions.Find(id));
            }
            Question question = db.Questions.Find(questionId);
            if (question == null)
                return HttpNotFound();

            return View(question);
        }
        public ActionResult NewQuestion(int? questionId, int? answerIndex, int? slidebarValue)
        {
            if (questionId == null)
            {
                return HttpNotFound();
            }
            Question question = db.Questions.Find(questionId);
            if (question == null)
                return HttpNotFound();


            if (question.Type == QuestionType.SlideBar)
                EcoCalculator.Calculate(question, (int)slidebarValue);
            else
                EcoCalculator.Calculate(question, question.Answers[(int)answerIndex]);


            Question nextQuestion = question.Type == QuestionType.Branching ?
                question.Answers[(int)answerIndex].NextQuestion :
                question.NextQuestion;
            if (nextQuestion == null)
                return RedirectToAction("EndResults");

            return RedirectToAction("Index", new { questionId = nextQuestion.ID });
        }
        public ActionResult EndResults()
        {
            return View(EcoCalculator.GetFinalResults());
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}