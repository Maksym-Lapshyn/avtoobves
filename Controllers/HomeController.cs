using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avtoobves.Infrastructure;
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

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetProducts(cancellationToken);

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Product(int id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProduct(id, cancellationToken);

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Blog(CancellationToken cancellationToken)
        {
            var blogPosts = await _blogPostRepository.GetBlogPosts(cancellationToken);
            
            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> BlogPost(Guid id, CancellationToken cancellationToken)
        {
            var blogPost = await _blogPostRepository.GetBlogPost(id, cancellationToken);

            return View(blogPost);
        }
        
        [HttpGet]
        public IActionResult Contacts() => View();
        
        [HttpGet]
        public IActionResult Categories() => View();

        [HttpGet]
        public async Task<IActionResult> Category(string categoryName, CancellationToken cancellationToken)
        {
            // TODO: filter in db
            var allProducts = await _productRepository.GetProducts(cancellationToken);
            var category = allProducts.Where(p => p.Category.ToString() == categoryName).ToList();

            return View(category);
        }
    }
}