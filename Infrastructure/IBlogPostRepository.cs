using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avtoobves.Models;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Infrastructure
{
    public interface IBlogPostRepository
    {
        Task<List<BlogPost>> GetBlogPosts(CancellationToken cancellationToken);

        Task<BlogPost> GetBlogPost(Guid id, CancellationToken cancellationToken);

        Task<BlogPost> DeleteBlogPost(Guid id);

        Task SaveBlogPost(BlogPost newBlogPost, IFormFile firstImage, IFormFile secondImage);
    }
} 