using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avtoobves.Infrastructure;
using Avtoobves.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public AdminController(IProductRepository productRepository, IBlogPostRepository blogPostRepository)
        {
            _productRepository = productRepository;
            _blogPostRepository = blogPostRepository;
        }

        [HttpGet]
        public ActionResult Index() => View();
        
        [HttpGet]
        public async Task<IActionResult> Products(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetProducts(cancellationToken);
            
            return View(products.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> BlogPosts(CancellationToken cancellationToken)
        {
            var blogPosts = await _blogPostRepository.GetBlogPosts(cancellationToken);
            
            return View(blogPosts.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return View(new Product());
            }

            var products = await _productRepository.GetProducts(cancellationToken);
            var product = products.FirstOrDefault(p => p.Id == id);
            
            return View(product ?? new Product());
        }
        
        [HttpGet]
        public async Task<IActionResult> EditBlogPost(Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return View(new BlogPost());
            }

            var blogPosts = await _blogPostRepository.GetBlogPosts(cancellationToken);
            var blogPost = blogPosts.FirstOrDefault(bp => bp.Id == id);
            
            return View(blogPost ?? new BlogPost());
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(Product product, IFormFile image)
        {
            // TODO: filter in the db
            var products = await _productRepository.GetProducts(CancellationToken.None);
            var productWithSameName = products.FirstOrDefault(p => p.Name == product.Name && p.Id != product.Id);

            if (productWithSameName != null)
            {
                ModelState.AddModelError("Name", "Товар с таким названием уже существует");
                
                return View("EditProduct", product);
            }

            if (!ModelState.IsValid)
            {
                return View("EditProduct", product);
            }

            await _productRepository.SaveProduct(product, image);
            
            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveBlogPost(BlogPost blogPost, IFormFile firstImage, IFormFile secondImage)
        {
            if (!ModelState.IsValid)
            {
                return View("EditBlogPost", blogPost);
            }

            await _blogPostRepository.SaveBlogPost(blogPost, firstImage, secondImage);
            
            return RedirectToAction("BlogPosts");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedProduct = await _productRepository.DeleteProduct(id);

            if (deletedProduct == null)
            {
                return NotFound();
            }

            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var deletedBlogPost = await _blogPostRepository.DeleteBlogPost(id);

            if (deletedBlogPost == null)
            {
                return NotFound();
            }

            return RedirectToAction("BlogPosts");
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.New = true;

            return View("EditProduct", new Product());
        }
        
        [HttpGet]
        public IActionResult CreateBlogPost()
        {
            ViewBag.New = true;

            return View("EditBlogPost", new BlogPost());
        }
    }
}