using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NuGet.Common;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PasswordChangeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Sisteme Kayıtlı Böyle Bir Mail Yok");
                return View(model);
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            //Token, ASP.NET Identity tarafından kriptografik olarak üretilen tek kullanımlık, geçici bir anahtardır.
            //Şifre değiştirme isteğinin gerçekten o kişiden geldiğini doğrular,Sadece o kullanıcının şifresini sıfırlar
            //O linkte TOKEN var
            //→ Token doğruysa şifre değiştirilebilir
            var passwordResetTokenLink = Url.Action("ResetPassword", "PasswordChange", new
            {
                userId = user.Id,
                Token = passwordResetToken
            }, HttpContext.Request.Scheme);

            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Notika Proje", "projectbrky123@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User",user.Email);
            mimeMessage.To.Add(mailboxAddressTo);

            string htmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f9f9f9; margin: 0; padding: 0; }}
                            .email-wrapper {{ width: 100%; background-color: #f9f9f9; padding: 40px 0; }}
                            .email-content {{ max-width: 500px; margin: 0 auto; background-color: #ffffff; border: 1px solid #eeeeee; border-radius: 8px; overflow: hidden; }}
                            .header {{ background-color: #66D0BC; padding: 25px; text-align: center; color: #ffffff; }}
                            .header h2 {{ margin: 0; letter-spacing: 2px; font-weight: 300; }}
                            .body-text {{ padding: 30px; color: #444444; line-height: 1.6; }}
                            .button-wrapper {{ text-align: center; padding: 20px 0; }}
                            .reset-button {{ background-color: #66D0BC; color: #ffffff !important; padding: 12px 25px; text-decoration: none; border-radius: 4px; font-weight: bold; display: inline-block; }}
                            .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #999999; }}
                            .divider {{ border-top: 1px solid #eeeeee; margin: 20px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='email-wrapper'>
                            <div class='email-content'>
                                <div class='header'>
                                    <h2>NOTIKA</h2>
                                </div>
                                <div class='body-text'>
                                    <p>Merhaba,</p>
                                    <p>Hesabın için bir şifre sıfırlama talebinde bulunuldu. Şifreni yenilemek için aşağıdaki butona tıklayabilirsin:</p>
                
                                    <div class='button-wrapper'>
                                        <a href='{passwordResetTokenLink}' class='reset-button'>Şifremi Yenile</a>
                                    </div>

                                    <p>Bu bağlantı <strong>30 dakika</strong> boyunca geçerlidir.</p>
                
                                    <div class='divider'></div>
                                    <p style='font-size: 12px; color: #888;'>Eğer bu talebi sen yapmadıysan bu e-postayı güvenle silebilirsin, hesabında herhangi bir değişiklik yapılmadı.</p>
                                </div>
                                <div class='footer'>
                                    &copy; 2026 Notika Platform | Tüm Hakları Saklıdır.
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;

            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Notika Identity Proje Şifre Sıfırlama Talebi";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 587, false); // ✅ smtp.gmail.com (@ değil .)
            smtpClient.Authenticate("projectbrky123@gmail.com", "dbcgxsnixoyclcku");
            smtpClient.Send(mimeMessage);    // ✅ Gönder
            smtpClient.Disconnect(true);     // ✅ Bağlantıyı kapat

            TempData["SendMailProcess"] = "Mail Başarıyla İletilmiştir Lütfen Kontrol Ediniz";
            return RedirectToAction("ForgetPassword");
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId,string Token)
        {
            var PasswordModel = new PasswordChangeViewModel()
            {
                UserId = userId,
                Token = Token,
            };
            return View(PasswordModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordChangeViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Bir Hata Oluştu");
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Şifreler eşleşmiyor,Lütfen Kontrol Ediniz");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Bir Hata Oluştu");
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifre Değiştirme İşleminiz Başarıyla Gerçekleşti,  Giriş Sayfasına Yönlendiriliyorsunuz";
                return RedirectToAction("ResetPassword");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
    }
}
//## Yani senin dediğin gibi:
//**1.Adım →**O kullanıcıya özel token üret, maile göm
//**2. Adım →** Kullanıcı linke tıklayınca `userId` ve `token` birlikte gelir
//**3. Adım →** `userId` ile kullanıcıyı bul, sonra token bu kullanıcıya ait mi kontrol et