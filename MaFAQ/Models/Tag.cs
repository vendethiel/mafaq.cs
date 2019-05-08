using System.Collections.Generic;
using System.Linq;

namespace MaFAQ.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        public List<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
        public List<Question> Questions => QuestionTags.Select(qt => qt.Question).ToList();

        public string Name { get; set; }
    }
}