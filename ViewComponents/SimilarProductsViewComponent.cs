using System;
using System.Linq;
using Avtoobves.Infrastructure;
using Avtoobves.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.ViewComponents
{
    public class SimilarProductsViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public SimilarProductsViewComponent(IRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke(int productId)
        {
            var product = _repository.Products.First(p => p.Id == productId);

            var products = _repository.Products
                .Where(p => p.Category == product.Category)
                .Shuffle(new Random())
                .ToList();

            return View(products);
        }
    }
}