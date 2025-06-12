using System;
using System.Collections.Generic;
using System.IO;
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
    public class BlogPostRepository : BaseRepository, IBlogPostRepository
    {
        public BlogPostRepository(Context context, IConfiguration configuration) : base(context, configuration)
        {
        }

        public Task<List<BlogPost>> GetBlogPosts(CancellationToken cancellationToken)
        {
            return Context.BlogPosts.ToListAsync(cancellationToken);
        }

        public async Task<BlogPost> GetBlogPost(Guid id, CancellationToken cancellationToken)
        {
            var blogPost = await Context.BlogPosts.FindAsync(id, cancellationToken);

            return blogPost;
        }

        public async Task<BlogPost> DeleteBlogPost(Guid id)
        {
            var post = await Context.BlogPosts.FindAsync(id);

            if (post == null)
            {
                return null;
            }

            await DeleteImagesAsync(GetFirstBlogPostImageName(post.Id), GetSecondBlogPostImageName(post.Id));
            Context.BlogPosts.Remove(post);
            await Context.SaveChangesAsync();

            return post;
        }

        public async Task SaveBlogPost(BlogPost newBlogPost, IFormFile firstImage, IFormFile secondImage)
        {
            using var s3Client = CreateS3Client();
            using var transferUtility = CreateTransferUtility(s3Client);
            var existingPost = await Context.BlogPosts.FindAsync(newBlogPost.Id);

            if (existingPost == default)
            {
                newBlogPost.Id = Guid.NewGuid();
                newBlogPost.CreatedAt = DateTime.UtcNow;
                newBlogPost.CreatedBy = "Администратор";
                var (firstImageUrl, secondImageUrl) = await UploadBlogPostImagesAsync(newBlogPost.Id, firstImage, secondImage, transferUtility);
                newBlogPost.FirstImageUrl = firstImageUrl;
                newBlogPost.SecondImageUrl = secondImageUrl;

                Context.BlogPosts.Add(newBlogPost);
                await Context.SaveChangesAsync();
            }
            else
            {
                var isReplacingImages = firstImage != default && secondImage != default;

                if (isReplacingImages)
                {
                    await DeleteImagesAsync(GetFirstBlogPostImageName(existingPost.Id), GetSecondBlogPostImageName(existingPost.Id));
                    var (firstImageUrl, secondImageUrl) = await UploadBlogPostImagesAsync(newBlogPost.Id, firstImage, secondImage, transferUtility);
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

                await Context.SaveChangesAsync();
            }
        }

        private static async Task<(string firstImageUrl, string secondImageUrl)> UploadBlogPostImagesAsync(
            Guid blogPostId,
            IFormFile firstImageFile,
            IFormFile secondImageFile,
            ITransferUtility transferUtility)
        {
            var firstImageName = GetFirstBlogPostImageName(blogPostId);
            using var firstImageStream = new MemoryStream();
            using var firstImage = await Image.LoadAsync(firstImageFile.OpenReadStream());
            
            firstImage.Mutate(x => x.Resize(1440, 1080));
            await firstImage.SaveAsync(firstImageStream, new JpegEncoder());
            await transferUtility.UploadAsync(firstImageStream, BucketName, firstImageName);
            
            var secondImageName = GetSecondBlogPostImageName(blogPostId);
            using var secondImageStream = new MemoryStream();
            using var secondImage = await Image.LoadAsync(secondImageFile.OpenReadStream());
            
            secondImage.Mutate(x => x.Resize(1440, 1080));
            await secondImage.SaveAsync(secondImageStream, new JpegEncoder());
            await transferUtility.UploadAsync(secondImageStream, BucketName, secondImageName);
            
            var firstImageUrl = $"{CdnAddress}/{firstImageName}";
            var secondImageUrl = $"{CdnAddress}/{secondImageName}";

            return (firstImageUrl, secondImageUrl);
        }

        private static string GetFirstBlogPostImageName(Guid blogPostId) => $"blogPosts/{blogPostId}_1.jpg";
        
        private static string GetSecondBlogPostImageName(Guid blogPostId) => $"blogPosts/{blogPostId}_2.jpg";
    }
} 