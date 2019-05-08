using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFAQ.Models;

namespace MyFAQ.Controllers
{
    public class AnswersController : BaseController
    {
        
        private readonly AnswerService _service;

        public AnswersController(AnswerService service, UserService userService) : base(userService)
        {
            _service = service;
        }
        
        [HttpPost]
        public IActionResult Post([FromForm] Answer answer)
        {
            // prevent logged in users from accessing the page
            if (!HasToken(HttpContext))
                return Unauthorized();

            var created = _service.Create(answer, new []{"Question", "Body"}, _user);
            if (created == null)
                return StatusCode(500);
            return RedirectToAction("Get", "Questions", new { id = created.Question.QuestionId });
        }

    }
}
