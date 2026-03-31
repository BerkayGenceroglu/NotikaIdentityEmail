using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class ActivationController : Controller
    {
        private readonly EmailContext _emailContext;

        public ActivationController(EmailContext emailContext)
        {
            _emailContext = emailContext;
        }

        [HttpGet]
        public async Task<IActionResult> UserActivationPage()
        {
            TempData.Keep("EmailMove");
            return View();
        }

        [HttpPost]
        public IActionResult UserActivationPage(EmailVerificationViewModel EmailVerificationViewModel)
        {
            var email = TempData.Peek("EmailMove")?.ToString();

            var userCode = _emailContext.Users.Where(x => x.Email == email).Select(y => y.ActivationCode).FirstOrDefault();

            if (userCode == EmailVerificationViewModel.CodeParameter)
            {
                var User = _emailContext.Users.Where(x => x.Email == email).FirstOrDefault();
                User.EmailConfirmed = true;
                _emailContext.SaveChanges();
                TempData["VerifySuccess"] = true;
                return RedirectToAction("SuccessRooutingPage");
            }

            return View();
        }
        public IActionResult SuccessRooutingPage()
        {
            if (TempData["VerifySuccess"] == null)
                return RedirectToAction("Index", "Home"); // direkt girişi engelle
            return View();
        }
    }
}
//Peek yöntemi, veriyi sözlükten okumanızı sağlar ancak veriyi silinmek üzere işaretlemez. Yani veriyi hem okumak hem de bir sonraki istekte hala ulaşılabilir kılmak istiyorsanız bunu kullanırsınız.