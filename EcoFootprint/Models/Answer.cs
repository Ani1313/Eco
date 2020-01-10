using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace EcoFootprint.Models
{
    public class Answer
    {
        public int ID { get; set; }
        [Required]
        public string TextAM { get; set; }
        [Required]
        public string TextEN { get; set; }
        public virtual Question Question { get; set; }
        public virtual Question NextQuestion { get; set; }
        public virtual Score Score { get; set; }
        public int? NQID { get; set; }

    }
}