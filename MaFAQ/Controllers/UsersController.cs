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
    [Route("api/users")]
    public class UsersController : BaseController
    {
        public UsersController(FaqContext context) : base(context)
        {
        }

        // Extra routes
        [HttpPost("token")]
        public async Task<IActionResult> GetUserByToken()
        {
            var token = Request.Form["token"];
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Token == token);

            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Username == username && m.Password == password);
            return Ok(user?.Token); /* if user is null, then so be it */
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            var username = Request.Form["username"].ToString();
            var password = Request.Form["password"].ToString();

            var existingUser = await _context.Users.SingleOrDefaultAsync(m => m.Username == username);
            if (existingUser != null || username.Trim() == "" || password.Trim() == "")
                return BadRequest();

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var user = new User()
            {
                Username = username,
                Password = password,
                IsAdmin = true,
                Token = token
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Token);
        }

        // Normal routes
        // GET: api/users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Password = "*****";
            return Ok(user);
        }

        /*
        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            RequireLoggedOut();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.IsAdmin = false;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }
        */

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var currentUser = await RequireUser();
            if (!currentUser.IsAdmin)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid || currentUser.UserId == id)
            { /* don't delete current user */
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null || user.IsAdmin)
            { /* don't delete admins */
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}