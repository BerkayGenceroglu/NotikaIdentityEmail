using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var RoleList = roles.Select(t => new RoleListViewModel()
            {
                Id = t.Id,
                RoleName = t.Name!
            }).ToList();
            return View(RoleList);
        }

        public async Task<IActionResult> RemoveRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(role!);
            TempData["RemoveProcess"] = "Silme İşlemi Başarılı";
            return RedirectToAction("RoleList");
        }
        //Yani gördüğün x.değeri tabloda GUID formatında, kodda string formatında — ikisi de aynı değer, sadece tipler farklı.

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = model.RoleName
            });
            if (result.Succeeded)
            {
                TempData["success"] = "Rol Atama İşleminiz Başarıyla gerçekleşmiştir";
                return RedirectToAction("CreateRole");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(new UpdateRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["UpdateProcess"] = "Rol Güncelleme İşleminiz Başarıyla gerçekleşmiştir";
                return RedirectToAction("UpdateRole");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllUserList()
        {
            var users = await _userManager.Users.ToListAsync();
            
            var UserList = users.Select(b => new UserViewModel()
            {
                RoleId = b.Id,
                Name = b.Name,
                Surname = b.Surname,
                UserName = b.UserName,
                Email = b.Email,
                IsActive = b.IsActive,
                UserId= b.Id
            }).ToList();

            return View(UserList);
        }

        public async Task<IActionResult> DeactivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user!.IsActive = false;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        public async Task<IActionResult> ActivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user!.IsActive = true;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AssignRolePage(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            TempData["userId"] = user.Id;
            List<RoleAssignViewModel> roleAssignModels = new List<RoleAssignViewModel>();
            foreach (var item in roles)
            {
                RoleAssignViewModel model = new RoleAssignViewModel();
                model.RoleId = item.Id;
                model.RoleName = item.Name;
                model.RoleExist = userRoles.Contains(item.Name);
                roleAssignModels.Add(model);
            }

            return View(roleAssignModels);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRolePage(List<RoleAssignViewModel> model)
        {
            var userId = TempData["userId"].ToString();
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
            {
                return NotFound();
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var item in model)
            {
                if (item.RoleExist && !userRoles.Contains(item.RoleName))
                {
                    await _userManager.AddToRoleAsync(user, item.RoleName);
                }
                else if (!item.RoleExist && userRoles.Contains(item.RoleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                }
                // Rol değişim işleminden sonra bunu çağırın:
                await _signInManager.RefreshSignInAsync(user);
                TempData["RoleAssignProcess"] = "Değişiklikler başarıyla Kaydedildi";
            }
            return RedirectToAction("AssignRolePage", new {id = userId });

        }
    }
}
