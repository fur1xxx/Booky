using Booky.DataAccess.Repository.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;
using System.Text.Json;
using Booky.Utility;
using Microsoft.AspNetCore.Authorization;
using Booky.DataAccess.Migrations;
using Microsoft.Identity.Client;

namespace BookyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            List<Product> objProductList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> categoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

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
                productVM.Product = unitOfWork.Product.Get(x => x.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    unitOfWork.Product.Update(productVM.Product);
                }
                unitOfWork.Save();

                string wwwRootPath = webHostEnvironment.WebRootPath;

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        // Use Path.Combine for creating directory paths
                        string productPathRelative = Path.Combine("images", "products", $"product-{productVM.Product.Id}");
                        string finalPath = Path.Combine(wwwRootPath, productPathRelative);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        // Store the image URL with forward slashes for cross-platform compatibility
                        string imageUrl = "/" + productPathRelative.Replace('\\', '/') + "/" + fileName;
                        
                        ProductImage productImage = new()
                        {
                            ImageUrl = imageUrl,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    unitOfWork.Product.Update(productVM.Product);
                    unitOfWork.Save();
                }

                TempData["success"] = "Product was created/updated successfully";
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

        public IActionResult DeleteImage(int imageId)
        {
            var imageToDelete = unitOfWork.ProductImage.Get(x => x.Id == imageId);
            int productId = imageToDelete.ProductId;

            if (!string.IsNullOrEmpty(imageToDelete.ImageUrl))
            {
                // Handle path correctly - the DB stores URLs with forward slashes
                string relativePath = imageToDelete.ImageUrl.TrimStart('/');
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, relativePath);

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                unitOfWork.ProductImage.Delete(imageToDelete);
                unitOfWork.Save();

                TempData["success"] = "Image was deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = unitOfWork.Product.Get(x => x.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Use Path.Combine for creating directory paths
            string productPathRelative = Path.Combine("images", "products", $"product-{id}");
            string finalPath = Path.Combine(webHostEnvironment.WebRootPath, productPathRelative);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);

                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            unitOfWork.Product.Delete(productToBeDeleted);
            unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion 
    }
}