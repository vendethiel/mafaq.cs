using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFAQ.Models;

namespace MyFAQ.Controllers
{
    public class BaseController : Controller
    {
        private UserService _userService;
        protected User _user;

        public BaseController(UserService userService)
        {
            _userService = userService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cookies = context.HttpContext.Request.Cookies;
            if (HasToken(context.HttpContext))
            {
                var user = _userService.GetByToken(cookies["MYFAQ_TOKEN"]);
                ViewBag.MyUser = user;
                _user = user;
            }

            base.OnActionExecuting(context);
        }

        protected bool HasToken(HttpContext context)
        {
            var cookies = context.Request.Cookies;
            return cookies.ContainsKey("MYFAQ_TOKEN");
        }

        protected void LoginWithToken(string token)
        {
            HttpContext.Response.Cookies.Append("MYFAQ_TOKEN", token);
        }
    }
}
