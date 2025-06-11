using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Models
{
    public interface IRepository
    {
        IEnumerable<Product> Products { get; }
        
        IEnumerable<BlogPost> BlogPosts { get; }

        Product DeleteProduct(int id);

        BlogPost DeleteBlogPost(Guid id);

        void SaveProduct(Product product, IFormFile image);
        
        void SaveBlogPost(BlogPost newBlogPost, IFormFile firstImage, IFormFile secondImage);

        int GetSimilarProducts(int productId, bool left, bool right);
    }
}