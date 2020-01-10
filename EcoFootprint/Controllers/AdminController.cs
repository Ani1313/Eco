using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcoFootprint.Models;
using EcoFootprint.Abstract;
using System.Data;
using System.Data.SqlClient;
using EcoFootprint.DAL;
using System.IO;
using System.Data.Entity;
using System.Net;

namespace EcoFootprint.Controllers
{
    public class AdminController : Controller
    {
        EcoFootprintContext db = new EcoFootprintContext();

        [HttpGet]
        public ActionResult Password()
        {
            return View(new PasswordModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(PasswordModel pass)
        {
            if (pass.Password.GetHashCode() == -514787019)
            {
                Session["SignedIn"] = true;
                return RedirectToAction("Index");
            }
            return View(new PasswordModel { WrongPassword = true });
        }
        public ActionResult Index()
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            return View(db.Questions.ToList());
        }
        [HttpGet]
        public ActionResult Count(QuestionType? qt)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            return View(new CountViewModel { questionType = qt ?? QuestionType.Օptional, AnswerCount = 2 });
        }
        [HttpPost]
        public ActionResult Count(CountViewModel model)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction("Add", new { model.questionType, answersCount = model.AnswerCount });
        }
        [HttpGet]
        public ActionResult Add(QuestionType questionType, int? answersCount)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            AdminViewModel model = new AdminViewModel
            {
                Questions = db.Questions.ToList(),
                NewQuestion = new Question
                {
                    ID = db.Questions.Count() == 0 ? 1 : db.Questions.Max(x => x.ID) + 1,
                    Type = questionType,
                    Answers = questionType == QuestionType.SlideBar ? null :
                        new List<Answer> { new Answer(), new Answer(), new Answer(), new Answer(), new Answer(), new Answer(), new Answer(), new Answer() },
                    Slidebar = questionType == QuestionType.SlideBar ? new Slidebar() : null
                }
            };
            if (questionType == QuestionType.Օptional)
                model.NewQuestion.NQID = model.NewQuestion.ID + 1;
            if (questionType != QuestionType.SlideBar)
            {
                model.NewQuestion.Answers = new List<Answer>();
                for (int i = 0; i < answersCount; i++)
                {
                    model.NewQuestion.Answers.Add(new Answer());
                    if (questionType == QuestionType.Branching)
                        model.NewQuestion.Answers[i].NQID = (model.NewQuestion.ID + 1 + i);
                }
            }
            return View("Add", model);

        }
        [HttpPost]
        public ActionResult Add(AdminViewModel model)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (!ModelState.IsValid) return View(model);

            Question question = model.NewQuestion;
            if (db.Questions.Where(q => q.ID == question.ID).Count() > 0)
            {
                model.Message = "A question with this ID has already exist!!!";
                return View(model);
            }

            string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
            string extension = Path.GetExtension(model.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            model.ImagePath = "~/Content/Images/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
            model.ImageFile.SaveAs(fileName);

            question.BackgroundImage = model.ImagePath;

            var questionInDb = db.Questions.Find(question.ID);

            Question nextQuestion = db.Questions.Find(question.NQID);
            if (nextQuestion != null)
            {
                db.Entry(questionInDb).Reference(r => r.NextQuestion).CurrentValue = nextQuestion;
                question.NQID = null;
                question.NextQuestion = nextQuestion;
            }

            db.Questions.Add(question);
            if (question.Type == QuestionType.Branching)
            {
                foreach (var a in question.Answers)
                {
                    Question nextQuest = db.Questions.Find(a.NQID);
                    if (nextQuest != null)
                    {
                        db.Entry(a).Reference(r => r.NextQuestion).CurrentValue = nextQuest;
                        a.NQID = null;
                        a.NextQuestion = nextQuest;
                    }
                }
            }


            db.SaveChanges();

            ModelState.Clear();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }
        [HttpPost]
        public ActionResult Edit(Question question)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (ModelState.IsValid)
            {
                var questionInDb = db.Questions.Find(question.ID);

                if (question.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(question.ImageFile.FileName);
                    string extension = Path.GetExtension(question.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string imagePath = "~/Content/Images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    question.ImageFile.SaveAs(fileName);

                    question.BackgroundImage = imagePath;
                }

                if (question.NQID == -1)
                {
                    db.Entry(questionInDb).Reference(r => r.NextQuestion).CurrentValue = null;
                    question.NQID = null;
                    question.NextQuestion = null;
                }
                else
                {
                    Question nextQuestion = db.Questions.Find(question.NQID);
                    if (nextQuestion != null)
                    {
                        db.Entry(questionInDb).Reference(r => r.NextQuestion).CurrentValue = nextQuestion;
                        question.NextQuestion = nextQuestion;
                        question.NQID = null;
                    }
                }
                if (question.Type == QuestionType.Branching)
                {
                    foreach (var a in question.Answers)
                    {
                        Answer answerInDb = db.Answers.Find(a.ID);
                        if (answerInDb == null)
                        {
                            db.Answers.Add(a);
                            answerInDb = a;
                        }
                        Question nextQuestion = db.Questions.Find(a.NQID);
                        if (nextQuestion != null)
                        {
                            a.NextQuestion = nextQuestion;
                            a.NQID = null;
                            db.Entry(answerInDb).Reference(r => r.NextQuestion).CurrentValue = nextQuestion;

                        }
                    }
                }


                db.Entry(questionInDb).CurrentValues.SetValues(question);
                db.Entry(questionInDb).State = EntityState.Modified;


                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(question);
        }
        public ActionResult Details(int? id)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (id == null)
            {
                return HttpNotFound();
            }
            return View(db.Questions.Find(id));
        }
        public ActionResult Delete(int? id)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (id == null)
            {
                return HttpNotFound();
            }


            var answers = db.Answers.Where(a => a.NextQuestion.ID == id);
            if (answers != null && answers.Count() != 0)
                foreach (Answer answer in answers)
                    db.Answers.Remove(answer);


            Question question = db.Questions.Find(id);
            if (question != null)
            {
                if (question.Answers != null && question.Answers.Count > 0)
                    db.Answers.RemoveRange(question.Answers);
                else if (question.Slidebar != null)
                    db.Slidebars.Remove(question.Slidebar);

                db.Questions.Remove(question);
                db.SaveChanges();

                string fullPath = Request.MapPath("~/Content/Images/" + question.BackgroundImage);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAnswer(int? answerIndex, int? questionId)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            if (answerIndex == null || questionId == null)
            {
                return HttpNotFound();
            }
            Question question = db.Questions.Find(questionId);
            if (question != null && answerIndex < question.Answers.Count)
            {

                Answer answer = question.Answers[(int)answerIndex];
                if (answer != null)
                {
                    db.Answers.Remove(db.Questions.Find(questionId).Answers[(int)answerIndex]);
                    db.SaveChanges();
                }
            }
            return View("Edit", db.Questions.Find(questionId));
        }
        public ActionResult AddAnswer(int? questionId)
        {
            if (Session["SignedIn"] == null || (bool)Session["SignedIn"] == false)
                return RedirectToAction("Password");

            db.Questions.Find(questionId).Answers.Add(new Answer());
            return View("Edit", db.Questions.Find(questionId));

        }
        public ActionResult DeleteAll()
        {
            foreach (var q in db.Questions)
            {
                string fullPath = Request.MapPath("~/Content/Images/" + q.BackgroundImage);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            db.Database.Delete();
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult RefreshDb()
        {
            foreach (var q in db.Questions)
            {
                if (q.NextQuestion != null) q.NQID = null;

            }
            var allAnswers = db.Questions.Select(q => q.Answers);
            foreach (var answers in allAnswers)
            {
                foreach (var a in answers)
                {
                    if (a.NextQuestion != null) a.NQID = null;
                }
            }
            db.SaveChanges();


            var questionNextQuestionPairs = db.Questions.
                Join(
                db.Questions,
                question => question.NQID
                , nextQuestion => nextQuestion.ID,
                (q, nq) => new {Question = q,NextQuestion = nq }
                );
            foreach (var qnq in questionNextQuestionPairs)
            {
                db.Entry(qnq.Question).Reference(r => r.NextQuestion).CurrentValue = qnq.NextQuestion;

            }
            db.SaveChanges();


            allAnswers = db.Questions.Select(q => q.Answers);
            foreach (var answers in allAnswers)
            {
                foreach (var a in answers)
                {
                    if (a.NQID != null)
                    {
                        var nextQuestion = db.Questions.Find(a.NQID);
                        if (nextQuestion != null)
                            db.Entry(a).Reference(r => r.NextQuestion).CurrentValue = nextQuestion;

                    }
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");

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