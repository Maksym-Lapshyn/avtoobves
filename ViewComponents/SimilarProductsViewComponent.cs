using System.Linq;
using System.Threading;
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

        public async Task<IViewComponentResult> InvokeAsync(int productId, bool left, bool right, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetProducts(cancellationToken);
            var product = products.First(p => p.Id == productId);

            var similarProducts = products
                .Where(p => p.Category == product.Category)
                .Skip(await _productRepository.GetSimilarProductIds(productId, left, right, cancellationToken))
                .Take(4)
                .ToList();

            return View(similarProducts);
        }
    }
}