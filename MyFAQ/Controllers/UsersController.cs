using Microsoft.AspNetCore.Mvc;
using MyFAQ.Models;

namespace MyFAQ.Controllers
{
    public class UsersController : BaseController
    {
        private readonly UserService _service;
        public UsersController(UserService service) : base(service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.All());
        }
        
        public IActionResult Login()
        {
            // prevent logged in users from accessing the page
            if (HasToken(HttpContext))
                return Unauthorized();
            
            if (HttpContext.Request.Method != "POST")
            {
                ViewBag.IsInvalid = false;
                return View();
            }

            var username = HttpContext.Request.Form["username"];
            var password = HttpContext.Request.Form["password"];
            string token;
            if (HttpContext.Request.Form.ContainsKey("login"))
            {
                ViewBag.IsLogin = true;
                token = _service.GetToken(username, password);
            }
            else
            {
                ViewBag.IsLogin = false;
                token = _service.Register(username, password);
            }

            if (token == null)
            {
                ViewBag.IsInvalid = true;
                return View(); /* incorrect login yada yada */
            }
            else
            {
                LoginWithToken(token);
                return Redirect("/"); /* redirect to homepage */
            }
        }

		[HttpGet]
        public IActionResult Get([FromRoute] int id)
        {
            return View(_service.Get(id));
        }
	}
}