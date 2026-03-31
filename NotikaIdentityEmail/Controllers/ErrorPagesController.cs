using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class ErrorPagesController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult ErrorPage(int statusCode)
        {
            if (statusCode == 404)
            {
                return RedirectToAction("Page404");
            }
            if (statusCode == 401)
            {
                return RedirectToAction("Page401");
            }
            if (statusCode == 403)
            {
                return RedirectToAction("Page403");

            }
            return View(statusCode);
        }
        public IActionResult Page404()
        {
            return View();
        }
        public IActionResult Page401()
        {
            return View();
        }
        public IActionResult Page403()
        {
            return View();
        }
    }
}
