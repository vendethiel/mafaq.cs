using System.Collections.Generic;
using System.Linq;

namespace MaFAQ.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public User Author { get; set; }
        public List<Answer> Answers { get; set; }
        public List<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();

        public List<Tag> Tags => QuestionTags.Select(qt => qt.Tag).ToList();
        public int AcceptedAnswerId { get; set; } /* Answer? */

        public string Title { get; set; }
        public string Body { get; set; }
    }
}