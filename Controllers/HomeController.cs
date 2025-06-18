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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Product(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetProduct(productId, cancellationToken);

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
        public IActionResult Contacts()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Categories()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string categoryName, CancellationToken cancellationToken)
        {
            // TODO: filter in db
            var allProducts = await _productRepository.GetProducts(cancellationToken);
            var category = allProducts.Where(p => p.Category.ToString() == categoryName).ToList();

            return View(category);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Parse the return URL to extract route values
            var uri = new Uri(returnUrl, UriKind.RelativeOrAbsolute);
            var path = uri.IsAbsoluteUri ? uri.AbsolutePath : returnUrl;
            
            // Remove leading slash and split path segments
            var segments = path.TrimStart('/').Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            
            // Remove existing culture prefix if present
            if (segments.Length > 0 && (segments[0] == "ru" || segments[0] == "uk"))
            {
                segments = segments.Skip(1).ToArray();
            }
            
            // Add new culture prefix
            segments = new[] { culture }.Concat(segments).ToArray();
            var newPath = "/" + string.Join("/", segments);
            
            return LocalRedirect(newPath);
        }
    }
}