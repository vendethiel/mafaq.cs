
using System.ComponentModel.DataAnnotations;

namespace MaFAQ.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        public Question Question { get; set; }
        public User Author { get; set; }

        public string Body { get; set; }
    }
}