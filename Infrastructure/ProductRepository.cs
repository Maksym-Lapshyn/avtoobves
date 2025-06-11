using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.S3.Transfer;
using Avtoobves.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Avtoobves.Infrastructure
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(Context context, IConfiguration configuration) 
            : base(context, configuration)
        {
        }

        public IEnumerable<Product> Products => Context.Products.ToList();

        public Product DeleteProduct(int id)
        {
            var product = Context.Products.Find(id);

            if (product == null)
            {
                return product;
            }

            DeleteImages(GetProductSmallImageName(product.Id), GetProductBigImageName(product.Id));
            Context.Products.Remove(product);
            Context.SaveChanges();

            return product;
        }

        public void SaveProduct(Product product, IFormFile image)
        {
            using var s3Client = CreateS3Client();
            using var transferUtility = CreateTransferUtility(s3Client);
            var existingProduct = Context.Products.Find(product.Id);

            if (existingProduct == default)
            {
                Context.Products.Add(product);
                Context.SaveChanges();
                
                var savedProduct = Context.Products.FirstOrDefault(p => p.Name == product.Name);
                var (bigImageUrl, smallImageUrl) = UploadProductImages(savedProduct.Id, image, transferUtility);
                savedProduct.BigImage = bigImageUrl;
                savedProduct.SmallImage = smallImageUrl;

                Context.SaveChanges();
                return;
            }
            
            if (existingProduct.BigImage != product.BigImage)
            {
                DeleteImages(GetProductSmallImageName(existingProduct.Id), GetProductBigImageName(existingProduct.Id));
                var (bigImageName, smallImageName) = UploadProductImages(product.Id, image, transferUtility);
                existingProduct.BigImage = bigImageName;
                existingProduct.SmallImage = smallImageName;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;

            Context.SaveChanges();
        }

        public int GetSimilarProducts(int productId, bool left, bool right)
        {
            var product = Context.Products.Find(productId);

            var products = Context
                .Products
                .Where(p => p.Category == product.Category)
                .ToList();

            var position = 0;

            for (var i = 0; i < products.Count; i++)
            {
                if (products[i].Id == productId)
                {
                    position = i;
                }
            }

            var indexOfFirstElement = 0;

            if (left)
            {
                position -= 4;
            }
            else if (right)
            {
                position += 4;
            }

            if (position < products.Count - 3)
            {
                indexOfFirstElement = position < 0 ? 0 : position;
            }
            else if (position >= products.Count - 3)
            {
                indexOfFirstElement = position - 3;
            }

            return indexOfFirstElement;
        }

        private static (string bigImageUrl, string smallImageUrl) UploadProductImages(int productId, IFormFile imageFile, ITransferUtility transferUtility)
        {
            var bigImageName = GetProductBigImageName(productId);
            var smallImageName = GetProductSmallImageName(productId);  
            using var bigImage = new MemoryStream();
            using var smallImage = new MemoryStream();
            using var originalImage = Image.Load(imageFile.OpenReadStream());
                
            originalImage.Mutate(x => x.Resize(1440, 1080));
            originalImage.Save(bigImage, new JpegEncoder());
            transferUtility.Upload(bigImage, BucketName, bigImageName);
            
            originalImage.Mutate(x => x.Resize(400, 300));
            originalImage.Save(smallImage, new JpegEncoder());
            transferUtility.Upload(smallImage, BucketName, smallImageName);

            var bigImageUrl = $"{CdnAddress}/{bigImageName}";
            var smallImageUrl = $"{CdnAddress}/{smallImageName}";

            return (bigImageUrl, smallImageUrl);
        }

        private static string GetProductBigImageName(int productId) => $"products/big_{productId}.jpg";
        
        private static string GetProductSmallImageName(int productId) => $"products/small_{productId}.jpg";
    }
} 