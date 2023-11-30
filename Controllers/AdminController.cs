using System;
using System.Linq;
using Avtoobves.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IRepository _repository;

        public AdminController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index() => View(_repository.Products);

        public ActionResult EditProduct(int id)
        {
            var product = _repository.Products.FirstOrDefault(p => p.Id == id);

            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product, IFormFile image)
        {
            var productWithSameName = _repository
                .Products
                .FirstOrDefault(p => p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase));

            if (productWithSameName != default)
            {
                ModelState.AddModelError("Name", "Product names cannot repeat!");
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            _repository.SaveProduct(product, image);

            TempData["success"] = $"Товар {product.Name} сохранен";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var productForDelete = _repository.DeleteProduct(id);

            if (productForDelete != null)
            {
                TempData["message"] = $"Товар {productForDelete.Name} удален";
            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateProduct()
        {
            ViewBag.New = true;

            return View("EditProduct", new Product());
        }
    }
}