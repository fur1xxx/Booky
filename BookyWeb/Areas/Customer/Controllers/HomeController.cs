using Booky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Booky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Booky.Utility;
using Microsoft.AspNetCore.Http;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfwork)
        {
            _logger = logger;
            this.unitOfWork = unitOfwork;   
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Count());
            }

            IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = unitOfWork.Product.Get(x => x.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart shoppingCartFromDb = unitOfWork.ShoppingCart.Get(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);

            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
                unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
                unitOfWork.Save();
            }
            else
            {
                //add shopping cart record
                unitOfWork.ShoppingCart.Add(shoppingCart);
                unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
            }

            TempData["success"] = "Cart was updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
