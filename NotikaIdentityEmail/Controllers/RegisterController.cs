using Humanizer;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using static System.Net.WebRequestMethods;

namespace NotikaIdentityEmail.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterUserViewModel model)
        {
            Random random = new Random();
            int code = random.Next(100000, 1000000);
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Şifreler eşleşmiyor,Lütfen Kontrol Ediniz");
                return View(model);
            }

            AppUser appUser = new AppUser()
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Username,
                Email = model.Email,
                ActivationCode = code
            };
            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (result.Succeeded)
            {
                MimeMessage mimeMessage = new MimeMessage();

                MailboxAddress mailboxAddressFrom = new MailboxAddress("Notika Proje", "projectbrky123@gmail.com");
                mimeMessage.From.Add(mailboxAddressFrom);

                MailboxAddress mailboxAddressTo = new MailboxAddress("User", model.Email);
                mimeMessage.To.Add(mailboxAddressTo);

                var htmlBody = $@"
                    <!DOCTYPE html>
                    <html lang='tr'>
                    <head><meta charset='UTF-8'/></head>
                    <body style='margin:0;padding:0;background-color:#f4f4f5;font-family:Segoe UI,sans-serif;'>
                      <table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;'>
                        <tr><td align='center'>
                          <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;'>

                            <tr>
                              <td style='background:linear-gradient(135deg,#4f46e5,#7c3aed);padding:36px 48px;text-align:center;'>
                                <div style='font-size:36px;margin-bottom:14px;'>✉️</div>
                                <h1 style='margin:0;color:#ffffff;font-size:22px;font-weight:700;'>E-posta Doğrulama</h1>
                                <p style='margin:8px 0 0;color:rgba(255,255,255,0.7);font-size:14px;'>Hesabınızı Aktive Etmek İçin Tek Adım Kaldı</p>
                              </td>
                            </tr>

                            <tr>
                              <td style='padding:40px 48px;'>
                                <p style='color:#374151;font-size:15px;'>Merhaba,</p>
                                <p style='color:#6b7280;font-size:14px;line-height:1.8;'>Hesabınızı doğrulamak için aşağıdaki aktivasyon kodunu ilgili alana giriniz.</p>
                                <table width='100%' cellpadding='0' cellspacing='0'>
                                  <tr>
                                    <td style='background:#f8f7ff;border:2px dashed #c4b5fd;border-radius:12px;padding:28px;text-align:center;'>
                                      <p style='margin:0 0 10px;font-size:11px;font-weight:600;letter-spacing:2.5px;color:#8b5cf6;text-transform:uppercase;'>Aktivasyon Kodunuz</p>
                                      <p style='margin:0;font-family:Courier New,monospace;font-size:38px;font-weight:700;color:#1f2937;letter-spacing:10px;'>{code}</p>
                                      <p style='margin:12px 0 0;font-size:12px;color:#9ca3af;'>Bu kod <strong>10 dakika</strong> geçerlidir.</p>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                            </tr>

                            <tr>
                              <td style='padding:0 48px 32px;'>
                                <table width='100%'><tr>
                                  <td style='background:#fffbeb;border-left:4px solid #f59e0b;border-radius:8px;padding:14px 18px;'>
                                    <p style='margin:0;font-size:13px;color:#92400e;'>⚠️ Bu isteği siz başlatmadıysanız bu e-postayı dikkate almayınız.</p>
                                  </td>
                                </tr></table>
                              </td>
                            </tr>

                            <tr>
                              <td style='background:#f9fafb;border-top:1px solid #f3f4f6;padding:20px 48px;text-align:center;'>
                                <p style='margin:0;font-size:12px;color:#9ca3af;'>Bu e-posta otomatik gönderilmiştir, lütfen yanıtlamayınız.<br/>© 2025 Notika — Tüm hakları saklıdır.</p>
                              </td>
                            </tr>

                          </table>
                        </td></tr>
                      </table>
                    </body>
                    </html>";
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = htmlBody;

                mimeMessage.Body =bodyBuilder.ToMessageBody();
                mimeMessage.Subject = "Notika Identity Proje Aktivasyon Kodu";

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Connect("smtp.gmail.com", 587, false); // ✅ smtp.gmail.com (@ değil .)
                smtpClient.Authenticate("projectbrky123@gmail.com", "dbcgxsnixoyclcku");
                smtpClient.Send(mimeMessage);    // ✅ Gönder
                smtpClient.Disconnect(true);     // ✅ Bağlantıyı kapat

                TempData["EmailMove"] = appUser.Email;
                return RedirectToAction("UserActivationPage", "Activation");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    //Bu hata herhangi bir input’a bağlı değil.Boş bırakmak tüm hataları ekrana bastrırıken kullanılmalıdır.
                    //O anki HTTP isteği boyunca yaşayan, form verilerini ve o verilere ait hataları tutan bir "sözlüktür"
                }
            }
            return View();
        }
    }
}
