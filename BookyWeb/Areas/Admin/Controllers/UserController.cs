using Booky.DataAccess.Data;
using Booky.DataAccess.Repository.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork unitOfWork;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        { 

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = unitOfWork.ApplicationUser.Get(x => x.Id ==  userId, includeProperties: "Company"),
                RoleList = roleManager.Roles.Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Name
                }),
                CompanyList = unitOfWork.Company.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            RoleVM.ApplicationUser.Role = userManager.GetRolesAsync(unitOfWork.ApplicationUser.Get(x => x.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();

            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string oldRole = userManager.GetRolesAsync(unitOfWork.ApplicationUser.Get(x => x.Id == roleManagementVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = unitOfWork.ApplicationUser.Get(x => x.Id == roleManagementVM.ApplicationUser.Id);

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                if (roleManagementVM.ApplicationUser.Role == SD.Role_User_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_User_Company)
                {
                    applicationUser.CompanyId = null;
                }

                unitOfWork.ApplicationUser.Update(applicationUser);
                unitOfWork.Save();

                userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_User_Company && applicationUser.CompanyId != roleManagementVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                    unitOfWork.ApplicationUser.Update(applicationUser);
                    unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<ApplicationUser> objApplicationUserList = unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in objApplicationUserList)
            { 
                user.Role = userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = objApplicationUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var userFromDb = unitOfWork.ApplicationUser.Get(x => x.Id == id);

            if (userFromDb == null)
            {
                return Json ( new {success = false, message = "Error while Locking/Unlocking"});
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked and we nned to unclock 
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1);
            }

            unitOfWork.ApplicationUser.Update(userFromDb);
            unitOfWork.Save();

            return Json(new { success = true, message = "Operation successful" });
        }
        #endregion 
    }
}
