using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Avtoobves.Models;

namespace Avtoobves.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProjectRepository _repository;

        public HomeController(IProjectRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult Category(string categoryName)
        {
            var products = _repository
                .Products
                .Where(p => p.Category.ToString() == categoryName)
                .ToList();
            
            return View(products);
        }

        public ActionResult Product(int productId)
        {
            var product = _repository
                .Products
                .FirstOrDefault(p => p.Id == productId);
            
            return View(product);
        }

        public ActionResult Contacts()
        {
            return View();
        }
    }
}