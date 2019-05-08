using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFAQ.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public User Author { get; set; }
        public List<Answer> Answers { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public int AcceptedAnswerId { get; set; } /* Answer? */

        public string Title { get; set; }
        public string Body { get; set; }
    }
}