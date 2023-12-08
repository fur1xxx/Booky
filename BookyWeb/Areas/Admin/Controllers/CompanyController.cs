using Booky.DataAccess.Repository.IRepository;
using Booky.Models;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Company> companyList = unitOfWork.Company.GetAll().ToList();

            return View(companyList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Company obj)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Company.Add(obj);

                unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Company companyObj = unitOfWork.Company.Get(x => x.Id == id);

            if (companyObj == null)
            {
                return NotFound();
            }

            return View(companyObj);
        }

        [HttpPost]
        public IActionResult Edit(Company obj)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Company.Update(obj);

                unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Company> objCompanyList = unitOfWork.Company.GetAll().ToList();

            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company companyToBeDeleted = unitOfWork.Company.Get(x => x.Id == id);

            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            unitOfWork.Company.Delete(companyToBeDeleted);

            unitOfWork.Save();

            return Json(new { success = true, message= "Delete successful" });
        }
        #endregion 
    }
}
