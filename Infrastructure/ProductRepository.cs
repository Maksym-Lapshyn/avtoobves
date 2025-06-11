using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using Avtoobves.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Product>> GetProducts(CancellationToken cancellationToken)
        {
            return await Context.Products.OrderBy(p => p.Category).ToListAsync(cancellationToken);
        }

        public async Task<Product> GetProduct(int id, CancellationToken cancellationToken)
        {
            var product = await Context.Products.FindAsync(id, cancellationToken);

            return product;
        }

        public async Task<Product> DeleteProduct(int id)
        {
            var product = await Context.Products.FindAsync(id);

            if (product == null)
            {
                return null;
            }

            await DeleteImagesAsync(GetProductSmallImageName(product.Id), GetProductBigImageName(product.Id));
            Context.Products.Remove(product);
            await Context.SaveChangesAsync();

            return product;
        }

        public async Task SaveProduct(Product product, IFormFile image)
        {
            using var s3Client = CreateS3Client();
            using var transferUtility = CreateTransferUtility(s3Client);
            var existingProduct = await Context.Products.FindAsync(product.Id);

            if (existingProduct == default)
            {
                Context.Products.Add(product);
                await Context.SaveChangesAsync();
                
                var savedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Name == product.Name);
                var (bigImageUrl, smallImageUrl) = await UploadProductImagesAsync(savedProduct.Id, image, transferUtility);
                savedProduct.BigImage = bigImageUrl;
                savedProduct.SmallImage = smallImageUrl;

                await Context.SaveChangesAsync();
                return;
            }
            
            if (existingProduct.BigImage != product.BigImage)
            {
                await DeleteImagesAsync(GetProductSmallImageName(existingProduct.Id), GetProductBigImageName(existingProduct.Id));
                var (bigImageName, smallImageName) = await UploadProductImagesAsync(product.Id, image, transferUtility);
                existingProduct.BigImage = bigImageName;
                existingProduct.SmallImage = smallImageName;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;

            await Context.SaveChangesAsync();
        }

        public async Task<int> GetSimilarProductIds(int productId, bool left, bool right, CancellationToken cancellationToken)
        {
            var product = await Context.Products.FindAsync(productId);

            var products = await Context
                .Products
                .Where(p => p.Category == product.Category)
                .ToListAsync(cancellationToken);

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

        private static async Task<(string bigImageUrl, string smallImageUrl)> UploadProductImagesAsync(
            int productId, 
            IFormFile imageFile, 
            ITransferUtility transferUtility)
        {
            var bigImageName = GetProductBigImageName(productId);
            var smallImageName = GetProductSmallImageName(productId);
            using var bigImage = new MemoryStream();
            using var smallImage = new MemoryStream();
            using var originalImage = await Image.LoadAsync(imageFile.OpenReadStream());
                
            originalImage.Mutate(x => x.Resize(1440, 1080));
            await originalImage.SaveAsync(bigImage, new JpegEncoder());
            await transferUtility.UploadAsync(bigImage, BucketName, bigImageName);
            
            originalImage.Mutate(x => x.Resize(400, 300));
            await originalImage.SaveAsync(smallImage, new JpegEncoder());
            await transferUtility.UploadAsync(smallImage, BucketName, smallImageName);

            var bigImageUrl = $"{CdnAddress}/{bigImageName}";
            var smallImageUrl = $"{CdnAddress}/{smallImageName}";

            return (bigImageUrl, smallImageUrl);
        }

        private static string GetProductBigImageName(int productId) => $"products/big_{productId}.jpg";
        
        private static string GetProductSmallImageName(int productId) => $"products/small_{productId}.jpg";
    }
} 