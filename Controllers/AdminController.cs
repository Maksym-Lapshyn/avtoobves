using System;
using System.Linq;
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

        public ActionResult Index() => View();
        
        public ActionResult Products() => View(_productRepository.Products.ToList());

        public ActionResult BlogPosts() => View(_blogPostRepository.BlogPosts.ToList());

        public ActionResult EditProduct(int? id)
        {
            var product = _productRepository.Products.FirstOrDefault(p => p.Id == id);
            return View(product ?? new Product());
        }
        
        public ActionResult EditBlogPost(Guid? id)
        {
            var blogPost = _blogPostRepository.BlogPosts.FirstOrDefault(bp => bp.Id == id);
            return View(blogPost ?? new BlogPost());
        }

        [HttpPost]
        public ActionResult SaveProduct(Product product, IFormFile image)
        {
            var productWithSameName = _productRepository
                .Products
                .FirstOrDefault(p => p.Name == product.Name && p.Id != product.Id);

            if (productWithSameName != null)
            {
                ModelState.AddModelError("Name", "Товар с таким названием уже существует");
                return View("EditProduct", product);
            }

            if (!ModelState.IsValid)
            {
                return View("EditProduct", product);
            }

            _productRepository.SaveProduct(product, image);

            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public ActionResult SaveBlogPost(BlogPost blogPost, IFormFile firstImage, IFormFile secondImage)
        {
            if (!ModelState.IsValid)
            {
                return View("EditBlogPost", blogPost);
            }

            _blogPostRepository.SaveBlogPost(blogPost, firstImage, secondImage);

            return RedirectToAction("BlogPosts");
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var deleteProduct = _productRepository.DeleteProduct(id);

            if (deleteProduct == null)
            {
                return NotFound();
            }

            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public ActionResult DeleteBlogPost(Guid id)
        {
            var deletedBlogPost = _blogPostRepository.DeleteBlogPost(id);

            if (deletedBlogPost == null)
            {
                return NotFound();
            }

            return RedirectToAction("BlogPosts");
        }

        public ActionResult CreateProduct()
        {
            ViewBag.New = true;

            return View("EditProduct", new Product());
        }
        
        public ActionResult CreateBlogPost()
        {
            ViewBag.New = true;

            return View("EditBlogPost", new BlogPost());
        }
    }
}