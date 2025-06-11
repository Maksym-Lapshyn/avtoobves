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

        public ActionResult Index() => View();
        
        public ActionResult Products() => View(_repository.Products.ToList());

        public ActionResult BlogPosts() => View(_repository.BlogPosts.ToList());

        public ActionResult EditProduct(int id)
        {
            var product = _repository.Products.FirstOrDefault(p => p.Id == id);

            return View(product);
        }
        
        public ActionResult EditBlogPost(Guid id)
        {
            var blogPost = _repository.BlogPosts.FirstOrDefault(bp => bp.Id == id);

            return View(blogPost);
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

            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public ActionResult EditBlogPost(BlogPost blogPost, IFormFile firstImage, IFormFile secondImage)
        {
            if (!ModelState.IsValid)
            {
                return View(blogPost);
            }

            _repository.SaveBlogPost(blogPost, firstImage, secondImage);

            TempData["success"] = $"Пост {blogPost.Title} сохранен";

            return RedirectToAction("BlogPosts");
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var deleteProduct = _repository.DeleteProduct(id);

            if (deleteProduct != null)
            {
                TempData["message"] = $"Товар {deleteProduct.Name} удален";
            }

            return RedirectToAction("Products");
        }
        
        [HttpPost]
        public ActionResult DeleteBlogPost(Guid id)
        {
            var deletedBlogPost = _repository.DeleteBlogPost(id);

            if (deletedBlogPost != null)
            {
                TempData["message"] = $"Пост {deletedBlogPost.Title} удален";
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