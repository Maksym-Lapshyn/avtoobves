using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Models
{
    public interface IRepository
    {
        IEnumerable<Product> Products { get; }

        Product DeleteProduct(int id);

        void SaveProduct(Product product, IFormFile image);

        int GetSimilarProducts(int productId, bool left, bool right);
    }
}