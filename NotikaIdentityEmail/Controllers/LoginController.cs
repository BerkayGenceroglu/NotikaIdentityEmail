using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSettingModel _jwtSettingModel;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext emailContext, UserManager<AppUser> userManager ,IOptions<JwtSettingModel> options )
        {
            _signInManager = signInManager;
            _emailContext = emailContext;
            _userManager = userManager;
            _jwtSettingModel = options.Value;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            // Kullanıcıyı bul
            var user = _emailContext.Users.FirstOrDefault(x => x.UserName == model.Username);

            if (user == null)
            {
                // Kullanıcı bulunamadıysa
                ModelState.AddModelError("", "Kullanıcı Adı veya Şifre Hatalı");
                return View();
            }

            SimpleUserViewModel simpleUserViewModel = new SimpleUserViewModel()
            {
                Name = user.Name,
                Surname = user.Surname,
                Id = user.Id,
                Email = user.Email,
                City = user.City,
                Username = user.UserName,
            };

            if (!user.EmailConfirmed)
            {
                // E-posta doğrulanmamışsa
                ModelState.AddModelError("", "E-posta adresinizi doğrulamanız gerekmektedir. Aksi takdirde giriş yapamazsınız.");
                return View();
            }

            if (user.IsActive == false)
            {
                ModelState.AddModelError("", "Kullanıcı Pasif Durumdadır,Daha Sonra Tekrar Deneyiniz");
                return View();
            }

            // Şifreyi kontrol et
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(simpleUserViewModel);
                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = true, // JavaScript tarafından erişilmesini engeller (XSS koruması)
                    Secure = true,   // Sadece HTTPS üzerinden gönderilir
                    SameSite = SameSiteMode.Strict, // CSRF saldırılarını önlemeye yardımcı olur
                    Expires = DateTime.UtcNow.AddHours(1) // Çerezin geçerlilik süresi
                });
                return RedirectToAction("EditProfile", "Profile");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı Adı veya Şifre Hatalı");
                return View();
            }


        }

        public string GenerateJwtToken(SimpleUserViewModel model)
        {
            var claim = new Claim[]
            {
                new Claim("Name",model.Name),
                new Claim("Surname",model.Surname),
                new Claim("City",model.City),
                new Claim("Username",model.Username),
                new Claim(ClaimTypes.Email,model.Email),
                new Claim(ClaimTypes.NameIdentifier,model.Id),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                //her token’i benzersiz yapmak.
                //Guid.NewGuid().ToString() ile her token’a eşsiz bir kimlik veriyoruz.
            };

            var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingModel.Key));

            var creds = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);
            //Token’i bu gizli anahtarla ve bu algoritmayla imzalayacağız
            var token = new JwtSecurityToken(
                issuer: _jwtSettingModel.Issuer,
                audience: _jwtSettingModel.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettingModel.ExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }



        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("UserLogin");
        }


        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Login", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet] 
        public async Task<IActionResult> ExternalLoginCallBack(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/"); 

            if (remoteError != null)
            {
                ModelState.AddModelError("", $"External Provider Error: {remoteError}");
                return RedirectToAction("UserLogin"); 
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
                return RedirectToAction("Inbox", "Message");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null) return RedirectToAction("UserLogin");

            var baseUsername = email.Split('@')[0];
            var username = baseUsername;
            int i = 1;

            while (await _userManager.FindByNameAsync(username) != null)
            {
                username = baseUsername + i;
                i++;
            }

            var user = new AppUser()
            {
                UserName = username,
                Email = email,
                Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                Surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User",
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Inbox", "Message");
            }

            return RedirectToAction("UserLogin");
        }
    }
}
