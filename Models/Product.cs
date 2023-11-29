using System.ComponentModel.DataAnnotations;

namespace Avtoobves.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter product's name!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter product's description!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Select product's category!")]
        public Category Category { get; set; }

        public string SmallImage { get; set; }

        public string BigImage { get; set; }
    }
}