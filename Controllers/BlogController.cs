using System.Threading;
using System.Threading.Tasks;
using Avtoobves.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var blogPosts = await _blogPostRepository.GetBlogPosts(cancellationToken);
            
            return View(blogPosts);
        }
    }
}