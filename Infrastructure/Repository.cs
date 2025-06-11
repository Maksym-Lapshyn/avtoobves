using System;
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
        
        public IEnumerable<BlogPost> BlogPosts => _context.BlogPosts.ToList();

        public Product DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return product;
            }
            
            using var s3Client = new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);

            DeleteImages(s3Client, GetProductSmallImageName(product.Id), GetProductBigImageName(product.Id));
            _context.Products.Remove(product);
            _context.SaveChanges();

            return product;
        }

        public BlogPost DeleteBlogPost(Guid id)
        {
            var post = _context.BlogPosts.Find(id);

            if (post == null)
            {
                return post;
            }
            
            using var s3Client = new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);

            DeleteImages(s3Client, GetFirstBlogPostImageName(post.Id), GetSecondBlogPostImageName(post.Id));
            _context.BlogPosts.Remove(post);
            _context.SaveChanges();

            return post;
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
                var (bigImageUrl, smallImageUrl) = UploadProductImages(savedProduct.Id, image, transferUtility);
                savedProduct.BigImage = bigImageUrl;
                savedProduct.SmallImage = smallImageUrl;

                _context.SaveChanges();

                return;
            }
            
            if (existingProduct.BigImage != product.BigImage)
            {
                DeleteImages(s3Client, GetProductSmallImageName(existingProduct.Id), GetProductBigImageName(existingProduct.Id));
                var (bigImageName, smallImageName) = UploadProductImages(product.Id, image, transferUtility);
                existingProduct.BigImage = bigImageName;
                existingProduct.SmallImage = smallImageName;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;

            _context.SaveChanges();
        }

        public void SaveBlogPost(BlogPost newBlogPost, IFormFile firstImage, IFormFile secondImage)
        {
            using var s3Client = new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);
            using var transferUtility = new TransferUtility(s3Client);
            var existingPost = _context.BlogPosts.Find(newBlogPost.Id);

            if (existingPost == default)
            {
                newBlogPost.Id = Guid.NewGuid();
                newBlogPost.CreatedAt = DateTime.UtcNow;
                newBlogPost.CreatedBy = "Администратор";
                var (firstImageUrl, secondImageUrl) = UploadBlogPostImages(newBlogPost.Id, firstImage, secondImage, transferUtility);
                newBlogPost.FirstImageUrl = firstImageUrl;
                newBlogPost.SecondImageUrl = secondImageUrl;

                _context.BlogPosts.Add(newBlogPost);
                _context.SaveChanges();
            }
            else
            {
                var isReplacingImages = firstImage != default && secondImage != default;

                if (isReplacingImages)
                {
                    DeleteImages(s3Client, GetFirstBlogPostImageName(existingPost.Id), GetSecondBlogPostImageName(existingPost.Id));
                    var (firstImageUrl, secondImageUrl) = UploadBlogPostImages(newBlogPost.Id, firstImage, secondImage, transferUtility);
                    existingPost.FirstImageUrl = firstImageUrl;
                    existingPost.SecondImageUrl = secondImageUrl;
                }
                
                existingPost.CreatedBy = newBlogPost.CreatedBy;
                existingPost.CreatedAt = newBlogPost.CreatedAt;
                existingPost.Title = newBlogPost.Title;
                existingPost.Description = newBlogPost.Description;
                existingPost.FirstParagraphText = newBlogPost.FirstParagraphText;
                existingPost.SecondParagraphText = newBlogPost.SecondParagraphText;
                existingPost.ThirdParagraphText = newBlogPost.ThirdParagraphText;

                _context.SaveChanges();
            }
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

        private static (string firstImageUrl, string secondImageUrl) UploadBlogPostImages(
            Guid blogPostId,
            IFormFile firstImageFile,
            IFormFile secondImageFile,
            ITransferUtility transferUtility)
        {
            var firstImageName = GetFirstBlogPostImageName(blogPostId);
            using var fistImageStream = new MemoryStream();
            using var firstImage = Image.Load(firstImageFile.OpenReadStream());
            
            firstImage.Mutate(x => x.Resize(1440, 1080));
            firstImage.Save(fistImageStream, new JpegEncoder());
            transferUtility.Upload(fistImageStream, BucketName, firstImageName);
            
            var secondImageName = GetSecondBlogPostImageName(blogPostId);
            using var secondImageStream = new MemoryStream();
            using var secondImage = Image.Load(secondImageFile.OpenReadStream());
            
            secondImage.Mutate(x => x.Resize(1440, 1080));
            secondImage.Save(secondImageStream, new JpegEncoder());
            transferUtility.Upload(secondImageStream, BucketName, secondImageName);
            
            var firstImageUrl = $"{CdnAddress}/{firstImageName}";
            var secondImageUrl = $"{CdnAddress}/{secondImageName}";

            return (firstImageUrl, secondImageUrl);
        }

        private static void DeleteImages(IAmazonS3 s3Client, params string[] imageNames)
        {
            if (imageNames == default)
            {
                return;
            }

            var imagesToDelete = imageNames
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => new KeyVersion { Key = i })
                .ToList();

            if (!imagesToDelete.Any())
            {
                return;
            }
            
            var deleteObjectRequest = new DeleteObjectsRequest
            {
                BucketName = BucketName,
                Objects = imagesToDelete
            };

            s3Client.DeleteObjectsAsync(deleteObjectRequest).GetAwaiter().GetResult();
        }
        
        private static string GetProductBigImageName(int productId) => $"products/big_{productId}.jpg";
        
        private static string GetProductSmallImageName(int productId) => $"products/small_{productId}.jpg";
        
        private static string GetFirstBlogPostImageName(Guid blogPostId) => $"blogPosts/{blogPostId}_1.jpg";
        
        private static string GetSecondBlogPostImageName(Guid blogPostId) => $"blogPosts/{blogPostId}_2.jpg";
    }
}