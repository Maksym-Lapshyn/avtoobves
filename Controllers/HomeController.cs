using System;
using System.Linq;
using Avtoobves.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index() => View();

        public ActionResult Categories() => View();

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

        public ActionResult Contacts() => View();

        public ActionResult Blog()
        {
            var blogPosts = _repository.BlogPosts.ToList();

            return View(blogPosts);
        }
        
        public ActionResult BlogPost(Guid id)
        {
            var blogPost = _repository
                .BlogPosts
                .FirstOrDefault(bp => bp.Id == id);

            return View(blogPost);
        }
    }
}