using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace EcoFootprint.Models
{
    public class CountViewModel
    {

        public QuestionType questionType { get; set; }
        [Range(2, 8)]
        public int AnswerCount { get; set; }

    }
}