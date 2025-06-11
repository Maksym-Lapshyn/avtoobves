using System.Collections.Generic;
using Avtoobves.Models;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Infrastructure
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }

        Product DeleteProduct(int id);

        void SaveProduct(Product product, IFormFile image);

        int GetSimilarProducts(int productId, bool left, bool right);
    }
} 