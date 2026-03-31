using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class CategoryController : Controller
    {
        private readonly EmailContext _emailContext;

        public CategoryController(EmailContext emailContext)
        {
            _emailContext = emailContext;
        }


        public async Task<IActionResult> CategoryList()
        {
            var token = Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("UserLogin","Login");
            }

            JwtSecurityToken jwt;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                jwt = handler.ReadJwtToken(token);
            }
            catch 
            {
                return RedirectToAction("UserLogin", "Login");

            }
            var city = jwt.Claims.FirstOrDefault(x => x.Type == "City").Value;

            if (city != "istanbul")
            {
                return Forbid();
            }

            var values = await _emailContext.Categories.ToListAsync();
            return View(values);
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            category.CategoryStatus = true;
            _emailContext.Categories.Add(category);
            _emailContext.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _emailContext.Categories.Find(id);
            _emailContext.Categories.Remove(category);
            _emailContext.SaveChanges();
            return RedirectToAction("CategoryList");
        }


        public IActionResult UpdateCategory(int id)
        {
            var category = _emailContext.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            category.CategoryStatus = true;
            _emailContext.Categories.Update(category);
            _emailContext.SaveChanges();
            return RedirectToAction("CategoryList");
        }

    }
}
