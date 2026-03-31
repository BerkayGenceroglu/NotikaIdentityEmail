using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents.NavBarViewComponents
{
    public class _MessageListOnNavbarComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _emailContext;

        public _MessageListOnNavbarComponentPartial(UserManager<AppUser> userManager, EmailContext emailContext)
        {
            _userManager = userManager;
            _emailContext = emailContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = await _emailContext.Messages
                .Where(y => y.ReceiverEmail == user.Email && y.IsRead == false)
                .Join(
                    _emailContext.Users,
                    message => message.SenderEmail,
                    user => user.Email,
                    (message, user) => new MessageWithImagesViewModel
                    {
                        MessageId = message.MessageId,  
                        MessageDetail = message.MessageDetail,
                        SenderEmail= message.SenderEmail,
                        UserImage = user.ImageUrl
                    }
                )
                .ToListAsync();
            return View(messages);
        }
    }
}
