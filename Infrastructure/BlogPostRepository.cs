using System;
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
    public class BlogPostRepository : BaseRepository, IBlogPostRepository
    {
        public BlogPostRepository(Context context, IConfiguration configuration) 
            : base(context, configuration)
        {
        }

        public IEnumerable<BlogPost> BlogPosts => Context.BlogPosts.ToList();

        public BlogPost DeleteBlogPost(Guid id)
        {
            var post = Context.BlogPosts.Find(id);

            if (post == null)
            {
                return post;
            }

            DeleteImages(GetFirstBlogPostImageName(post.Id), GetSecondBlogPostImageName(post.Id));
            Context.BlogPosts.Remove(post);
            Context.SaveChanges();

            return post;
        }

        public void SaveBlogPost(BlogPost newBlogPost, IFormFile firstImage, IFormFile secondImage)
        {
            using var s3Client = CreateS3Client();
            using var transferUtility = CreateTransferUtility(s3Client);
            var existingPost = Context.BlogPosts.Find(newBlogPost.Id);

            if (existingPost == default)
            {
                newBlogPost.Id = Guid.NewGuid();
                newBlogPost.CreatedAt = DateTime.UtcNow;
                newBlogPost.CreatedBy = "Администратор";
                var (firstImageUrl, secondImageUrl) = UploadBlogPostImages(newBlogPost.Id, firstImage, secondImage, transferUtility);
                newBlogPost.FirstImageUrl = firstImageUrl;
                newBlogPost.SecondImageUrl = secondImageUrl;

                Context.BlogPosts.Add(newBlogPost);
                Context.SaveChanges();
            }
            else
            {
                var isReplacingImages = firstImage != default && secondImage != default;

                if (isReplacingImages)
                {
                    DeleteImages(GetFirstBlogPostImageName(existingPost.Id), GetSecondBlogPostImageName(existingPost.Id));
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

                Context.SaveChanges();
            }
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

        private static string GetFirstBlogPostImageName(Guid blogPostId) => $"blogPosts/{blogPostId}_1.jpg";
        
        private static string GetSecondBlogPostImageName(Guid blogPostId) => $"blogPosts/{blogPostId}_2.jpg";
    }
} 