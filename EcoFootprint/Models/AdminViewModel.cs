using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcoFootprint.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EcoFootprint.Models
{
    public class AdminViewModel
    {
        public string Message { get; set; }
        public Question NewQuestion { get; set; }
        public List<Question> Questions { get; set; }
        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }
}