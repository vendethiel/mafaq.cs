using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaFAQ.Models;

namespace MaFAQ.Controllers
{
    [Produces("application/json")]
    [Route("api/questions")]
    public class QuestionsController : BaseController
    {
        public QuestionsController(FaqContext context) : base(context)
        {
        }

        // Extra routes
        // GET: api/questions/count
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _context.Questions.CountAsync();
            return Ok(count);
        }

        // GET: api/questions/pages
        [HttpGet("pages")]
        public async Task<IActionResult> Pages()
        {
            var count = await _context.Questions.CountAsync();
            return Ok(Paginator.Pages(count));
        }

        // POST: api/questions/5/tags
        [HttpPost("{id}/tag/{tagName}")]
        public async Task<IActionResult> AddTag([FromRoute] int id, [FromRoute] string tagName)
        {
			var question = await _context.Questions.Include(q => q.Author).SingleOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.SingleOrDefaultAsync(m => m.Name == tagName);
            if (tag == null)
            {
                tag = new Tag() { Name = tagName};
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync(); // need to save now to get an ID
            }

            var questionTag = new QuestionTag() { Question = question, Tag = tag };
            _context.Add(questionTag);

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/questions/5/answers
        [HttpGet("{id}/answers")]
        public async Task<IActionResult> GetAnswers([FromRoute] int id)
        {
            var question = await _context.Questions.SingleOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return Ok(question.Answers);
        }
        
        // Normal routes
        // GET: api/questions
        [HttpGet]
        public IEnumerable<Question> GetQuestions()
        {
            var ctx = _context.Questions.Include(q => q.Author);
            if (HttpContext.Request.Query.TryGetValue("page", out var page))
				return ctx.Paginate(int.Parse(page));
            else
				return ctx;
        }

        // GET: api/questions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion([FromRoute] int id)
        {
			var question = await _context.Questions
			    .Include(q => q.Author)
			    .Include(q => q.Answers)
			      .ThenInclude(a => a.Author)
			    .SingleOrDefaultAsync(m => m.QuestionId == id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // PUT: api/questions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion([FromRoute] int id, [FromBody] Question question)
        {
            var currentUser = await RequireUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var currentQuestion = _context.Find<Question>(id);
            if (id != question.QuestionId
             || currentQuestion == null
             || currentQuestion.Author.UserId != question.Author.UserId)
            { /* can't update a non-existing question, can't change author id */
                return BadRequest();
            }

            if (!currentUser.IsAdmin && currentQuestion.Author.UserId != currentUser.UserId)
            { /* can only update own questions, unless admin */
                return Unauthorized();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/questions
        [HttpPost]
        public async Task<IActionResult> PostQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            question.Author = await RequireUser(); /* force author */
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestion", new { id = question.QuestionId }, question);
        }

        // DELETE: api/questions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.Questions.SingleOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            var currentUser = await RequireUser();
            if (!currentUser.IsAdmin && question.Author.UserId != currentUser.UserId)
            { /* can only delete own questions, unless admin */
                return Unauthorized();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(question);
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionId == id);
        }
    }
}