using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web;

namespace EcoFootprint.Models
{

    public enum QuestionCategory
    {
        None,
        Food,
        Housing,
        Goods,
        Seruices,
        Mobility
    }
    public enum QuestionType
    {
        None, Օptional, Branching, SlideBar
    }

    public class Question
    {
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

        public int ID { get; set; }
        [Required]
        public QuestionType Type { get; set; }
        [Required]

        public QuestionCategory Category { get; set; }
        [Required]
        public string TextAM { get; set; }
        [Required]
        public string TextEN { get; set; }
        public string BackgroundImage { get; set; }
        public string InfoTextAM { get; set; }
		public string InfoTextEN { get; set; }
		public virtual Question NextQuestion { get; set; }
        public virtual List<Answer> Answers { get; set; }
        public virtual Slidebar Slidebar { get; set; }

        public int? NQID { get; set; }
    }
}