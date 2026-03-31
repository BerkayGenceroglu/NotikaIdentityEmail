using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents.NavBarViewComponents
{
    public class _NotificationListOnNavbarComponentPartial : ViewComponent
    {
        private readonly EmailContext _emailContext;

        public _NotificationListOnNavbarComponentPartial(EmailContext emailContext)
        {
            _emailContext = emailContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notificationList = await _emailContext.Notifications.ToListAsync();
            return View(notificationList);
        }
    }
}
