using Booky.DataAccess.Data;
using Booky.Models;
using Microsoft.AspNetCore.Mvc;
using Booky.DataAccess.Repository;
using Booky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Booky.Utility;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = unitOfWork.Category.GetAll().ToList();

            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if (obj.Name != null && obj.Name.ToLower() == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "The Display order can't exactly math the name");
            //}

            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(obj);

                unitOfWork.Save();

                TempData["success"] = "Category was created successfully";

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

            Category categoryObj = unitOfWork.Category.Get(x => x.Id == id);
            //Category categoryObj = dbContext.Categories.FirstOrDefault(x => x.Id == id);
            //Category categoryObj = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault(); 

            if (categoryObj == null)
            {
                return NotFound();
            }

            return View(categoryObj);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(obj);
                unitOfWork.Save();

                TempData["success"] = "Category was updated successfully";

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category categoryObj = unitOfWork.Category.Get(x => x.Id == id);
            //Category categoryObj = dbContext.Categories.FirstOrDefault(x => x.Id == id);
            //Category categoryObj = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault(); 

            if (categoryObj == null)
            {
                return NotFound();
            }

            return View(categoryObj);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(Category obj)
        {
            unitOfWork.Category.Delete(obj);
            unitOfWork.Save();

            TempData["success"] = "Category was deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
