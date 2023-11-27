using Booky.DataAccess.Repository.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;   
        }

        public IActionResult Index()
        {
            List<Product> objProductList = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {

            IEnumerable<SelectListItem> categoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            //ViewBag.categoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;
            ProductVM productVM = new()
            {
                CategoryList = categoryList,
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = unitOfWork.Product.Get(x => x.Id == id);
                return View(productVM); 
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);    
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product was created successfully";
                }
                else
                {
                    unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product was updated successfully";
                }

                unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                return View(productVM);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            //var jsonOptions = new JsonSerializerOptions
            //{
            //    ReferenceHandler = ReferenceHandler.Preserve,
            //    // Other serializator's parameters
            //};

            return Json(new {data = objProductList});   
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = unitOfWork.Product.Get(x => x.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);   
            }

            unitOfWork.Product.Delete(productToBeDeleted);

            unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion 
    }
}
