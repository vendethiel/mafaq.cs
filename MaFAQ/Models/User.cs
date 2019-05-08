using System.Collections.Generic;

namespace MaFAQ.Models
{
    public class User
    {
        public int UserId { get; set; }

        public List<Question> Questions { get; set;  }
        public List<Answer> Answers { get; set;  }

        public bool IsAdmin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}