using System.ComponentModel.DataAnnotations;

namespace Avtoobves.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(256)]
        [Required(ErrorMessage = "Enter product's RU name!")]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required(ErrorMessage = "Enter product's UA name!")]
        public string NameUk { get; set; }

        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter product's RU description!")]
        public string Description { get; set; }

        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter product's UA description!")]
        public string DescriptionUk { get; set; }

        [Required(ErrorMessage = "Select product's category!")]
        public Category Category { get; set; }

        [MaxLength(256)]
        public string SmallImage { get; set; }

        [MaxLength(256)]
        public string BigImage { get; set; }
    }
}