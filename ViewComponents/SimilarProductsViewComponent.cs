using System.Linq;
using System.Threading.Tasks;
using Avtoobves.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.ViewComponents
{
    public class SimilarProductsViewComponent : ViewComponent
    {
        private readonly IProductRepository _productRepository;

        public SimilarProductsViewComponent(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<IViewComponentResult> InvokeAsync(int productId, bool left, bool right)
        {
            var product = _productRepository.Products.First(p => p.Id == productId);

            var products = _productRepository.Products
                .Where(p => p.Category == product.Category)
                .Skip(_productRepository.GetSimilarProducts(productId, left, right))
                .Take(4)
                .ToList();

            return Task.FromResult<IViewComponentResult>(View(products));
        }
    }
}