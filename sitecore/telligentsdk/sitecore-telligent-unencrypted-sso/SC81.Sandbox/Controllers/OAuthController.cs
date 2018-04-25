using SC81.Sandbox.Authentication;
using SC81.Sandbox.Models;
using System;
using System.Web.Mvc;

namespace SC81.Sandbox.Controllers
{
    [Route(Name = "external")]
    public class OAuthController : Controller
    {
        // GET: Login
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;

            var model = new LoginBoxModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginBoxModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
                return this.View(model);

            string domain = TelligentSSOManager.ExtranetDomain;
            string username = model.UserName;
            if (!string.IsNullOrEmpty(domain) && !String.IsNullOrWhiteSpace(username) && !username.Contains("\\"))
                username = domain + "\\" + username;

            if (!Sitecore.Security.Authentication.AuthenticationManager.Login(username, model.Password))
            {
                ModelState.AddModelError("LoginInvalid", "Login invalid");
                return this.View(model);
            }

            TelligentSSOManager.Login(username);

            if (!string.IsNullOrEmpty(returnUrl))
                return this.Redirect(returnUrl);

            return this.Redirect("/");

        }

        public ActionResult Logout(string returnUrl)
        {
            TelligentSSOManager.Logout();
            return this.Redirect(returnUrl);
        }

    }
}