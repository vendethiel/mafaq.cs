using System.Collections.Generic;

namespace MyFAQ.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        public List<Tag> Tags { get; } = new List<Tag>();

        public string Name { get; set; }
    }
}