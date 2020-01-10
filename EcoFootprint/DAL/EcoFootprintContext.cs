using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using EcoFootprint.Models;

namespace EcoFootprint.DAL
{
    public class EcoFootprintContext:DbContext
    {
        public EcoFootprintContext():base("EcoFootprint")
        {

        }

       public DbSet<Question> Questions { get; set; }
       public DbSet<Answer> Answers { get; set; }
        public DbSet<Slidebar> Slidebars { get; set; }

    }
}