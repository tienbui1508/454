using _454.DataAccess.Data;
using _454.DataAccess.Repository;
using _454.DataAccess.Repository.IRepository;
using _454.Models;
using _454.Models.ViewModels;
using _454.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _454Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            
            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
          

            if (id == null || id == 0)
            {
                //create
                return View(new Company());

            } 
            else
            {
                //update
                Company companyObj = _unitOfWork.Company.Get(u=>u.Id==id);
                return View(companyObj);

            }

        }
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
               
                
                if (companyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObj);

                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);

                }
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            } else
            {
                return View(companyObj);

            }
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (companyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(companyFromDb);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.Company.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Company deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data = objCompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.Company.Get(u=>u.Id == id);
            if(companyToBeDeleted == null){
                return Json(new { success = false, message = "Error while deleting" });
            }

         

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });

        }

        #endregion
    }
}
