using System.ComponentModel.DataAnnotations;
namespace EcoFootprint.Models
{
    public class Slidebar
    {
        public int ID { get; set; }
        [Required]
        public string MinSideTextAM { get; set; }
        [Required]
        public string MinSideTextEN { get; set; }
        [Required]
        public string MaxSideTextAM { get; set; }
        [Required]
        public string MaxSideTextEN { get; set; }
        [Required]
        public virtual Score MinScore { get; set; }
        [Required]
        public virtual Score MaxScore { get; set; }

    }
}