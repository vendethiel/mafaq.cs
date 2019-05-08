using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MyFAQ.Models;

namespace MyFAQ.Controllers
{
    public class QuestionsController : BaseController
    {
        private readonly QuestionService _service;

        public QuestionsController(QuestionService service, UserService userService) : base(userService)
        {
            _service = service;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            int page = 1;
            if (HttpContext.Request.Query.TryGetValue("page", out var queryPage))
                page = Math.Max(1, int.Parse(queryPage));

            ViewBag.Pages = new ModelPaginator<Question>(_service).Generate(page);

            var models = _service.All(page);
            if (models == null)
                return NotFound();
            return View(models);
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Post([FromForm] Question question)
        {
			// prevent logged in users from accessing the page
            if (!HasToken(HttpContext))
                return Unauthorized();

			var created = _service.Create(question, new []{"Title", "Body"}, _user);
            if (created == null)
                return StatusCode(500);
            return RedirectToAction(nameof(Get), "Questions", new { id = created.QuestionId });
        }
        
        // TODO check the route has the correct "questions/" prefix
        [HttpGet("/questions/{id}")]
        //[HttpGet]
        public IActionResult Get([FromRoute] int id)
        {
			return View(_service.Get(id));
        }
    }
}
