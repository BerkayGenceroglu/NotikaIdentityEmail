using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userInfo = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditViewModel model = new UserEditViewModel()
            {
                UserName = userInfo.UserName,
                Name = userInfo.Name,
                Surname = userInfo.Surname,
                City = userInfo.City,
                ImageUrl = userInfo.ImageUrl,
                PhoneNumber = userInfo.PhoneNumber,
                Email = userInfo.Email,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            if (model.Passowrd != model.PasswordConfirm)
            {
                ModelState.AddModelError("PasswordConfirm", "Şifreler eşleşmiyor, Lütfen Kontrol Ediniz");
                return View(model);
            }

            var updateUser = await _userManager.GetUserAsync(User);
            if (updateUser == null) return RedirectToAction("Login", "Account");

            updateUser.Name = model.Name;
            updateUser.Surname = model.Surname;
            updateUser.UserName = model.UserName;
            updateUser.PhoneNumber = model.PhoneNumber;
            updateUser.Email = model.Email;
            updateUser.ImageUrl = model.ImageUrl;
            updateUser.City = model.City;

            if (!string.IsNullOrEmpty(model.Passowrd))
            {
                updateUser.PasswordHash = _userManager.PasswordHasher.HashPassword(updateUser, model.Passowrd);
            }

            var result = await _userManager.UpdateAsync(updateUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }
            else
            {
                TempData["success"] = true;
                // Kendi sayfasına yönlendir ki sayfa temizlensin ve TempData tetiklensin
                return RedirectToAction("EditProfile");
            }


        }

        
    }
}
