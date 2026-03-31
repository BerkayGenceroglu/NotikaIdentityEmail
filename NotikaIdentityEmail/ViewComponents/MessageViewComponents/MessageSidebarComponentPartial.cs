using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class MessageSidebarComponentPartial: ViewComponent
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;

        public MessageSidebarComponentPartial(EmailContext emailContext, UserManager<AppUser> userManager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.SendMessageCount = _emailContext.Messages.Where(x => x.SenderEmail == user.Email && x.IsDraft == false).Count();
            ViewBag.ReceiveMessageCount = _emailContext.Messages.Where(y => y.ReceiverEmail == user.Email && y.IsRead == false && y.IsDeleted ==false).Count();
            ViewBag.TrashBoxMessageCount = _emailContext.Messages.Where(y => y.ReceiverEmail == user.Email && y.IsDeleted == true).Count();
            ViewBag.DraftBoxMessageCount = _emailContext.Messages.Where(y => y.SenderEmail == user.Email && y.IsDraft == true).Count();
            return View();
        }
    }
}
