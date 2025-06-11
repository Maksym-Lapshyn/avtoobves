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

        public IActionResult Index() => View(_blogPostRepository.BlogPosts);
    }
}