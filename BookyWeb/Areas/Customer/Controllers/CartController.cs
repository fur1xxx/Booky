using Booky.DataAccess.Repository.IRepository;
using Booky.Models;
using Booky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfwork)
        {
            this.unitOfWork = unitOfwork;
        }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult Plus(int cartId)
        {
            var shoppingCartFromDb = unitOfWork.ShoppingCart.Get(x => x.Id == cartId);

            shoppingCartFromDb.Count += 1;
            unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var shoppingCartFromDb = unitOfWork.ShoppingCart.Get(x => x.Id == cartId);

            if (shoppingCartFromDb.Count <= 1)
            {
                //remove item from shopping cart
                unitOfWork.ShoppingCart.Delete(shoppingCartFromDb);
            }
            else
            {
                shoppingCartFromDb.Count -= 1;
                unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            }

            unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var shoppingCartFromDb = unitOfWork.ShoppingCart.Get(x => x.Id == cartId);

            unitOfWork.ShoppingCart.Delete(shoppingCartFromDb);

            unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.PriceForFifty;
                }
                else
                {
                    return shoppingCart.Product.PriceForOneHundred;
                }
            }
        }
    }
}
