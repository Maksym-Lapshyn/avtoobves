using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Avtoobves.Models
{
    public class Repository : IRepository
    {
        private const string BucketName = "avtoobves-images";
        private const string CdnAddress = "https://d1ucqhcti31ovy.cloudfront.net";
        
        private readonly AWSCredentials _awsCredentials;
        private readonly Context _context;

        public Repository(Context context, IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AwsCredentials");
            var accessKey = awsSection.GetValue<string>("AccessKey");
            var secretKey = awsSection.GetValue<string>("SecretKey");
            _awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            _context = context;
        }

        public IEnumerable<Product> Products => _context.Products.ToList();

        public Product DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return product;
            }
            
            using var s3Client = new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);

            DeleteImages(product, s3Client);
            _context.Products.Remove(product);
            _context.SaveChanges();

            return product;
        }

        public void SaveProduct(Product product, IFormFile image)
        {
            using var s3Client = new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);
            using var transferUtility = new TransferUtility(s3Client);
            var existingProduct = _context.Products.Find(product.Id);

            if (existingProduct == default)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                
                var savedProduct = _context.Products.FirstOrDefault(p => p.Name == product.Name);
                var (bigImageUrl, smallImageUrl) = UploadImages(savedProduct.Id, image, transferUtility);
                savedProduct.BigImage = bigImageUrl;
                savedProduct.SmallImage = smallImageUrl;

                _context.SaveChanges();

                return;
            }
            
            if (existingProduct.BigImage != product.BigImage)
            {
                DeleteImages(existingProduct, s3Client);
                
                var (bigImageName, smallImageName) = UploadImages(product.Id, image, transferUtility);
                existingProduct.BigImage = bigImageName;
                existingProduct.SmallImage = smallImageName;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;

            _context.SaveChanges();
        }

        public int GetSimilarProducts(int productId, bool left, bool right)
        {
            var product = _context.Products.Find(productId);

            var products = _context
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

        private static (string bigImageUrl, string smallImageUrl) UploadImages(int productId, IFormFile imageFile, ITransferUtility transferUtility)
        {
            var bigImageName = GetBigImageName(productId);
            var smallImageName = GetSmallImageName(productId);  
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

        private static void DeleteImages(Product product, IAmazonS3 s3Client)
        {
            var hasNoImages = string.IsNullOrWhiteSpace(product.SmallImage) && string.IsNullOrWhiteSpace(product.BigImage);

            if (hasNoImages)
            {
                return;
            }
            
            var deleteObjectRequest = new DeleteObjectsRequest
            {
                BucketName = BucketName,
                Objects = new List<KeyVersion>
                {
                    new()
                    {
                        Key = GetBigImageName(product.Id)
                    },
                    new()
                    {
                        Key = GetSmallImageName(product.Id)
                    }
                }
            };

            s3Client.DeleteObjectsAsync(deleteObjectRequest).GetAwaiter().GetResult();
        }
        
        private static string GetBigImageName(int productId) => $"products/big_{productId}.jpg";
        
        private static string GetSmallImageName(int productId) => $"products/small_{productId}.jpg";
    }
}