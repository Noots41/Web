using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Models;
using Helpers;
using Model;
using System.Web.Security;
using NHibernate;

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = CheckUser(model.Login, model.Password);
            
            if (result)
            {
                
                FormsAuthentication.SetAuthCookie(model.Login, true);
                return RedirectToLocal(returnUrl);

            }
            ModelState.AddModelError("", "Неудачная попытка входа.");
            return View(model);
        }

        private bool CheckUser(string login, string password)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var count = session.QueryOver<Author>().And(x => x.Login == login && x.Password == password).RowCount();
                return count == 1;
            }
        }


       
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Author user = null;
                using (var session = NHibernateHelper.OpenSession())
                {
                    user = session.QueryOver<Author>().Where(u => u.Login == model.Login).SingleOrDefault();
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (var session = NHibernateHelper.OpenSession())
                    {
                        //db.Users.Add(new User { Email = model.Name, Password = model.Password, Age = model.Age });
                        //db.SaveChanges();
                        //user = db.Users.Where(u => u.Email == model.Name && u.Password == model.Password).FirstOrDefault();
                        //ITransaction tx = session.BeginTransaction();
                        //var sql = session.CreateSQLQuery("INSERT into Author(Id, FirstName, LastName, Login, Password) select i.Id, i.FirstName, i.LastName, i.Login, i.Password from Author i").ExecuteUpdate();
                        //tx.Commit();
                        //user = session.QueryOver<Author>().Where(u => u.Login == model.Login && u.Password == model.Password).SingleOrDefault();
                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            try
                            {
                                var newUser = new Author { FirstName = model.FirstName, LastName = model.LastName, Login = model.Login, Password = model.Password };
                                session.Save(newUser);
                                transaction.Commit();
                            }
                            catch (HibernateException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

       
      
   
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}