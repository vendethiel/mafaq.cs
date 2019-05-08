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
    [Route("api/tags")]
    public class TagsController : BaseController
    {
        public TagsController(FaqContext context) : base(context)
        {
        }

        // Extra routes
        // the "/tag/" URL prefix indicates by-name tag actions
        // GET: api/tag/tagged/hereisthenameofthetag/questions
        [HttpGet("tagged/{tagName}/questions")]
        public async Task<IActionResult> QuestionsByTag([FromRoute]string tagName)
        {
            var tag = await _context.Tags.SingleOrDefaultAsync(m => m.Name == tagName);
            if (tag == null)
                return NotFound();
            return Ok(tag.Questions);
        }

        // Normal routes
        // GET: api/tags
        [HttpGet]
        public IEnumerable<Tag> GetTags()
        {
            return _context.Tags;
        }

        // GET: api/tags/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _context.Tags.SingleOrDefaultAsync(m => m.TagId == id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }
        
        // POST: api/tags
        [HttpPost]
        public async Task<IActionResult> PostTag([FromBody] Tag tag)
        {
            await RequireAdmin();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.TagId }, tag);
        }

        // DELETE: api/tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag([FromRoute] int id)
        {
            await RequireAdmin();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _context.Tags.SingleOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return Ok(tag);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }
}