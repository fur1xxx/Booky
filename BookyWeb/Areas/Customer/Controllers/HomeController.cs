using Booky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Booky.DataAccess.Repository.IRepository;

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
            IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productObj = unitOfWork.Product.Get(x => x.Id == id, includeProperties: "Category");   

            return View(productObj);
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
