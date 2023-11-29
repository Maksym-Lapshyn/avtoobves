using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Avtoobves.Models
{
    public interface IProjectRepository
    {
        IEnumerable<Product> Products { get; }
        
        Product DeleteProduct(int id);

        void SaveProduct(Product product, IFormFile image);

        int GetSimilarProducts(int productId, bool left, bool right);
    }
    
    public class ProjectRepository : IProjectRepository
    {
        public const string TempImage = "~/Images/temp.jpg";
        
        private readonly ProjectContext _context;
        private readonly IWebHostEnvironment _env;

        public ProjectRepository(ProjectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IEnumerable<Product> Products => _context.Products;

        public Product DeleteProduct(int id)
        {
            var productForDelete = _context.Products.Find(id);

            if (productForDelete == null)
            {
                return productForDelete;
            }

            DeleteImages(productForDelete);
            _context.Products.Remove(productForDelete);
            _context.SaveChanges();
            
            return productForDelete;
        }

        public void SaveProduct(Product product, IFormFile image)
        {
            if (image != default)
            {
                var oldImageLocation = Path.Combine(_env.ContentRootPath, "TempImage");
                using Stream fileStream = new FileStream(oldImageLocation, FileMode.Create);

                image.CopyToAsync(fileStream).GetAwaiter().GetResult();
                
                product.BigImage = TempImage;
            }
            
            if (product.Id == 0)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                
                product = _context.Products.FirstOrDefault(p => p.Name == product.Name);
                product.BigImage = ResizeBigImage(product.Id);
                product.SmallImage = SaveSmallImage(product.Id);
                
                _context.SaveChanges();
                
                return;
            }
            
            var forSave = _context.Products.Find(product.Id);
            
            if (forSave != null)
            {
                if (forSave.BigImage != product.BigImage)
                {
                    DeleteImages(forSave);
                    
                    forSave.BigImage = ResizeBigImage(product.Id);
                    forSave.SmallImage = SaveSmallImage(product.Id);
                }

                forSave.Name = product.Name;
                forSave.Description = product.Description;
                forSave.Category = product.Category;
            }
            
            _context.SaveChanges();
        }

        private string ResizeBigImage(int productId)
        {
            var oldImageLocation = Path.Combine(_env.ContentRootPath, "TempImage");
            var oldImage = new Bitmap(oldImageLocation);
            var newSize = new Size(1440, 1080);
            var resizedImage = new Bitmap(oldImage, newSize);
            
            oldImage.Dispose();
            File.Delete(oldImageLocation);
            resizedImage.Save(Path.Combine(_env.ContentRootPath, $"~/Images/big_{productId}.jpg"), ImageFormat.Jpeg);
            resizedImage.Dispose();
            
            return $"big_{productId}.jpg";
        }

        private string SaveSmallImage(int productId)
        {
            var bigImageLocation = Path.Combine(_env.ContentRootPath, $"~/Images/big_{productId}.jpg");
            var bigImage = new Bitmap(bigImageLocation);
            var newSize = new Size(400, 300);
            var smallImage = new Bitmap(bigImage, newSize);
            
            smallImage.Save(Path.Combine(_env.ContentRootPath, $"~/Images/small_{productId}.jpg"), ImageFormat.Jpeg);
            smallImage.Dispose();
            bigImage.Dispose();
            
            return $"small_{productId}.jpg";
        }

        private void DeleteImages(Product product)
        {
            var smallPath = Path.Combine(_env.ContentRootPath, $"~/Images/small_{product.Id}.jpg");
            var bigPath = Path.Combine(_env.ContentRootPath, $"~/Images/big_{product.Id}.jpg");
            
            if (File.Exists(smallPath))
            {
                File.Delete(smallPath);
            }
            
            if (File.Exists(bigPath))
            {
                File.Delete(bigPath);
            }
        }

        public int GetSimilarProducts(int productId, bool left, bool right)
        {
            var product = _context.Products.Find(productId);
            var repository = _context.Products.Where(p => p.Category == product.Category).ToList();
            var position = 0;
            
            for (var i = 0; i < repository.Count; i++)
            {
                if (repository[i].Id == productId)
                {
                    position = i;
                }
            }
            
            var indexOfFirstElement = 0;
            
            if (left == true)
            {
                position -= 4;
            }
            else if (right == true)
            {
                position += 4;
            }
            
            if (position < repository.Count - 3)
            {
                if (position < 0)
                {
                    indexOfFirstElement = 0;
                }
                else
                {
                    indexOfFirstElement = position;
                }
            }
            else if (position >= repository.Count - 3)
            {
                indexOfFirstElement = position - 3;
            }
            
            return indexOfFirstElement;
        }
    }
}