using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class CategorySidebarMessageComponentPartial : ViewComponent
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;

        public CategorySidebarMessageComponentPartial(EmailContext emailContext, UserManager<AppUser> userManager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = _emailContext.Categories.Where(x => x.CategoryStatus == true)
             .Select(y => new CategoryWithCountViewModel
             {
                 CategoryId = y.CategoryId,
                 CategoryName = y.CategoryName,
                 CategoryIcon = y.CategoryIcon,
                 MessageCount = _emailContext.Messages.Where(w => w.ReceiverEmail == user.Email).Count(c => c.CategoryId == y.CategoryId),
             }).ToList();
            return View(values);
        }
    }
}
