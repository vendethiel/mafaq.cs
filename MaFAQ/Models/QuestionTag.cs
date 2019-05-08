namespace MaFAQ.Models
{
    public class QuestionTag
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}