using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avtoobves.Models;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Infrastructure
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProducts(CancellationToken cancellationToken);

        Task<Product> GetProduct(int id, CancellationToken cancellationToken);
        
        Task<Product> DeleteProduct(int id);

        Task SaveProduct(Product product, IFormFile image);

        Task<int> GetSimilarProductIds(int productId, bool left, bool right, CancellationToken cancellationToken);
    }
} 