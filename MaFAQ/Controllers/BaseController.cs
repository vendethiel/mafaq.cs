using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaFAQ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MaFAQ.Controllers
{
    /**
     * This exception is used to short-circuit controller actions when an unauthorized action was attempted.
     * This is just to avoid having to deal with ASP.Net Core's Authentication stuff which is too complex for us.
     */
    public class MyOwnUnauthorized : Exception
    { }

    /**
     * Massages a MyOwnUnauthorized exception into an actual HTTP Response
     */
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is MyOwnUnauthorized)
                context.Result = new UnauthorizedResult();
        }
    }

    [CustomExceptionFilter]
    public class BaseController : Controller
    {
        protected readonly FaqContext _context;

        public BaseController(FaqContext context)
        {
            _context = context;
        }

        public async Task<User> RequireUser()
        {
            // TODO header Authorization: Bearer <TOKEN>
            if (!HttpContext.Request.Headers.ContainsKey("X-Token"))
                throw new MyOwnUnauthorized();
            var token = HttpContext.Request.Headers["X-Token"];
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Token == token);
            if (user == null)
                throw new MyOwnUnauthorized();
            return user;
        }

        public async Task<User> RequireAdmin()
        {
            var user = await RequireUser();
            if (!user.IsAdmin)
                throw new MyOwnUnauthorized();
            return user;
        }

        public void RequireLoggedOut()
        {
            if (HttpContext.Request.Headers.ContainsKey("X-Token"))
                throw new MyOwnUnauthorized();
        }
    }


}
