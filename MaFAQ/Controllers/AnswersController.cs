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
    [Route("api/answers")]
    public class AnswersController : BaseController
    {
        public AnswersController(FaqContext context) : base(context)
        {
        }

        // GET: api/answers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswer([FromRoute] int id)
        {
            var answer = await _context.Answers.SingleOrDefaultAsync(m => m.AnswerId == id);

            if (answer == null)
            {
                return NotFound();
            }

            return Ok(answer);
        }

        // PUT: api/answers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnswer([FromRoute] int id, [FromBody] Answer answer)
        {
            var currentUser = await RequireUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentAnswer = _context.Find<Answer>(id);
            if (id != answer.AnswerId
                || currentAnswer == null
                || currentAnswer.Author.UserId != answer.Author.UserId)
            { /* can't update a non-existing question, can't change author id */
                return BadRequest();
            }
            
            if (!currentUser.IsAdmin && currentAnswer.Author.UserId != currentUser.UserId)
            { /* can only update own questions, unless admin */
                return Unauthorized();
            }

            _context.Entry(answer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(id))
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

        // POST: api/answers
        [HttpPost]
        public async Task<IActionResult> PostAnswer([FromBody] Answer answer)
        {
            var currentUser = await RequireUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var question = await _context.Questions
                .Include(q => q.Answers)
                    .ThenInclude(a => a.Author)
                .SingleOrDefaultAsync(m => m.QuestionId == answer.Question.QuestionId);
            if (question == null)
            {
                return BadRequest();
            }

            var hasAnswerInQuestion = question.Answers.Exists((a) => a.Author.UserId == currentUser.UserId);
            if (hasAnswerInQuestion)
            { /* can only answer once */
                return BadRequest();
            }

            answer.Author = currentUser;
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnswer", new { id = answer.AnswerId }, answer);
        }

        // DELETE: api/answers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer([FromRoute] int id)
        {
            var currentUser = await RequireUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var answer = await _context.Answers.SingleOrDefaultAsync(m => m.AnswerId == id);
            if (answer == null)
            {
                return NotFound();
            }

            if (!currentUser.IsAdmin && answer.Author.UserId != currentUser.UserId)
            { /* can only update own questions, unless admin */
                return Unauthorized();
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return Ok(answer);
        }

        private bool AnswerExists(int id)
        {
            return _context.Answers.Any(e => e.AnswerId == id);
        }
    }
}