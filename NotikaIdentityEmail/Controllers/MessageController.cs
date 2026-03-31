using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext emailContext, UserManager<AppUser> userManager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }

        // Kullanıcı bilgisini merkezi bir yerden alalım
        private async Task<string> GetUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user?.Email;
        }

        // Sidebar sayılarının tek bir sorguyla (veya optimize) doldurulması
        private async Task FillSidebarCounts(string email)
        {
            ViewBag.InboxCount = await _emailContext.Messages.CountAsync(x => x.ReceiverEmail == email && !x.IsDeleted && !x.IsDraft && !x.IsRead);
            ViewBag.SendboxCount = await _emailContext.Messages.CountAsync(x => x.SenderEmail == email && !x.IsDraft && !x.IsDeleted);
            ViewBag.DraftBoxCount = await _emailContext.Messages.CountAsync(x => x.SenderEmail == email && x.IsDraft && !x.IsDeleted);
            ViewBag.TrashboxCount = await _emailContext.Messages.CountAsync(x => (x.ReceiverEmail == email || x.SenderEmail == email) && x.IsDeleted);
        }

        // Kategorileri Dropdown için hazırlayan yardımcı metot
        private async Task FillCategoryList()
        {
            var categories = await _emailContext.Categories.ToListAsync();
            ViewBag.CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.CategoryId.ToString()
            }).ToList();
        }

        public async Task<IActionResult> Inbox()
        {
            string email = await GetUser();
            await FillSidebarCounts(email);
            var values = await _emailContext.Messages.Include(x => x.Category)
                .Where(y => y.ReceiverEmail == email && !y.IsDeleted && !y.IsDraft)
                .OrderByDescending(c => c.SendDate).ToListAsync();
            return View(values);
        }

        public async Task<IActionResult> Sendbox()
        {
            string email = await GetUser();
            await FillSidebarCounts(email);
            var values = await _emailContext.Messages.Include(x => x.Category)
                .Where(y => y.SenderEmail == email && !y.IsDraft && !y.IsDeleted)
                .OrderByDescending(c => c.SendDate).ToListAsync();
            return View(values);
        }

        public async Task<IActionResult> DraftBox()
        {
            string email = await GetUser();
            await FillSidebarCounts(email);
            var values = await _emailContext.Messages.Include(x => x.Category)
                .Where(y => y.SenderEmail == email && y.IsDraft && !y.IsDeleted)
                .OrderByDescending(c => c.SendDate).ToListAsync();
            return View(values);
        }

        public async Task<IActionResult> Trashbox()
        {
            string email = await GetUser();
            await FillSidebarCounts(email);
            var values = await _emailContext.Messages.Include(x => x.Category)
                .Where(y => (y.ReceiverEmail == email || y.SenderEmail == email) && y.IsDeleted)
                .OrderByDescending(c => c.SendDate).ToListAsync();
            return View(values);
        }

        public IActionResult ChangeToMessageIsReadFalse(int messageId)
        {
            var message = _emailContext.Messages.FirstOrDefault(x => x.MessageId == messageId);
            if (message != null)
            {
                message.IsRead = false;
                _emailContext.Update(message); // Değişikliği EF'e bildir
                _emailContext.SaveChanges();   // Veritabanına kaydet
            }
            return RedirectToAction("Inbox");
        }

        public IActionResult ChangeToMessageIsReadTrue(int messageId)
        {
            var message = _emailContext.Messages.FirstOrDefault(x => x.MessageId == messageId);
            if (message != null)
            {
                message.IsRead = true;
                _emailContext.Update(message);
                _emailContext.SaveChanges();
            }
            return RedirectToAction("Inbox");
        }

        [HttpGet]
        public async Task<IActionResult> ComposeMessage()
        {
            await FillCategoryList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ComposeMessage(Message message, string buttonAction)
        {
            message.SenderEmail = await GetUser();
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            message.IsDeleted = false;
            message.IsDraft = (buttonAction == "draft");

            _emailContext.Messages.Add(message);
            await _emailContext.SaveChangesAsync();

            TempData["Success"] = message.IsDraft ? "Taslak Başarıyla kaydedildi." : "Mesaj Başarıyla gönderildi.";
            return RedirectToAction("ComposeMessage");
        }

        [HttpGet]
        public async Task<IActionResult> DrafMessage(int messageId)
        {
            await FillCategoryList(); // Kategorileri doldurur
            var messageDetail = await _emailContext.Messages.FindAsync(messageId);
            if (messageDetail == null) return NotFound();
            return View(messageDetail);
        }

        [HttpPost]
        public async Task<IActionResult> DrafMessage(Message message, string buttonAction)
        {
            var existingMessage = await _emailContext.Messages.FindAsync(message.MessageId);
            if (existingMessage == null) return NotFound();

            existingMessage.ReceiverEmail = message.ReceiverEmail;
            existingMessage.Subject = message.Subject;
            existingMessage.MessageDetail = message.MessageDetail;
            existingMessage.CategoryId = message.CategoryId;
            existingMessage.SendDate = DateTime.Now;

            // Buton aksiyonuna göre taslak durumunu güncelle
            if (buttonAction == "send")
            {
                existingMessage.IsDraft = false;
                TempData["ProcessSuccess"] = "Mesaj başarıyla gönderildi.";
            }
            else
            {
                existingMessage.IsDraft = true;
                TempData["ProcessSuccess"] = "Taslak başarıyla güncellendi.";
            }

            _emailContext.Update(existingMessage);
            await _emailContext.SaveChangesAsync();

            // Eğer mesaj gönderildiyse Giden Kutusu'na, taslak güncellendiyse Taslaklar'a gitmek daha mantıklıdır.
            // Ama mevcut sayfada kalmak istiyorsan:
            return RedirectToAction("DrafMessage", new { messageId = message.MessageId });
        }

        public async Task<IActionResult> MoveToTrash(int messageId)
        {
            var message = await _emailContext.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsDeleted = true;
                await _emailContext.SaveChangesAsync();
            }
            return RedirectToAction("Inbox");
        }

        public IActionResult MessageDetail(int messageId)
        {
            var messageDetail = _emailContext.Messages.Include(x => x.Category).Where(y => y.MessageId == messageId).FirstOrDefault();
            return View(messageDetail);
        }

        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var message = _emailContext.Messages.Find(messageId);
            _emailContext.Messages.Remove(message);
            _emailContext.SaveChanges();
            return RedirectToAction("Trashbox");
        }

        public async Task<IActionResult> GetMessageListByCategory(int CategoryId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = _emailContext.Messages.Include(x => x.Category).Where(y => y.ReceiverEmail == user.Email && y.CategoryId == CategoryId).OrderByDescending(c => c.SendDate).ToList();
            return View(values);
        }
    }
}