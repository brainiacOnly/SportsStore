using System.Web.Mvc;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthProvider authProvider;

        public AccountController(IAuthProvider auth)
        {
            authProvider = auth;
        }

        [HttpGet]
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.UserName, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("List", "Product"));
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username of password");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public RedirectResult Logout()
        {
            authProvider.Logout();
            return Redirect(Url.Action("List","Product"));
        }

        public PartialViewResult LoginButton()
        {
            if (authProvider.IsAuthenticated(User))
            {
                ViewBag.Identity = true;
            }
            else
            {
                ViewBag.Identity = false;
            }
            return PartialView();
        }
    }
}