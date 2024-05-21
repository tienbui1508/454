using _454.DataAccess.Data;
using _454.DataAccess.Repository;
using _454.DataAccess.Repository.IRepository;
using _454.Models;
using _454.Models.ViewModels;
using _454.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _454Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i=> new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u=>u.Id==userId)).GetAwaiter().GetResult().FirstOrDefault();

            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleVM)
        {
            
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleVM.ApplicationUser.Id);

            if (!(roleVM.ApplicationUser.Role == oldRole))
            {
                if (roleVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }
            
            return RedirectToAction("Index");

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach(var user in objUserList)
            {
               
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if(user.Company == null) {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new {data = objUserList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {


            return Json(new { success = true, message = "Delete successful" });

        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
                _unitOfWork.ApplicationUser.Update(objFromDb);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Unlock successful" });

            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
                _unitOfWork.ApplicationUser.Update(objFromDb);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Lock successful" });

            }
        }

        #endregion
    }
}
