using System;
using System.Collections.Generic;
using Avtoobves.Models;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Infrastructure
{
    public interface IBlogPostRepository
    {
        IEnumerable<BlogPost> BlogPosts { get; }

        BlogPost DeleteBlogPost(Guid id);

        void SaveBlogPost(BlogPost newBlogPost, IFormFile firstImage, IFormFile secondImage);
    }
} 