using System.Linq;
using Avtoobves.Infrastructure;
using Avtoobves.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public HomeController(IProductRepository productRepository, IBlogPostRepository blogPostRepository)
        {
            _productRepository = productRepository;
            _blogPostRepository = blogPostRepository;
        }

        public IActionResult Index()
        {
            var products = _productRepository
                .Products
                .OrderBy(p => p.Category)
                .ToList();

            return View(products);
        }

        public IActionResult Product(int id)
        {
            var product = _productRepository
                .Products
                .FirstOrDefault(p => p.Id == id);

            return View(product);
        }

        public IActionResult Blog()
        {
            var blogPosts = _blogPostRepository.BlogPosts.ToList();

            return View(blogPosts);
        }

        public IActionResult BlogPost(string id)
        {
            var blogPost = _blogPostRepository
                .BlogPosts
                .FirstOrDefault(bp => bp.Id.ToString() == id);

            return View(blogPost);
        }
    }
}