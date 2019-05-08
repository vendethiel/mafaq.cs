
using System.ComponentModel.DataAnnotations;

namespace MyFAQ.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }

        public Question Question { get; set; }
        public User Author { get; set; }

        public string Body { get; set; }
    }
}