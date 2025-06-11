using Avtoobves.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avtoobves.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository _repository;

        public BlogController(IRepository repository)
        {
            _repository = repository;
        }
        
        
    }
}