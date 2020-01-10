using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoFootprint.Models;
namespace EcoFootprint.Abstract
{
    interface IQuestions
    {
        IQueryable<Question> Questions{get; }
    }
}
