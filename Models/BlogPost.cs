using System;
using System.ComponentModel.DataAnnotations;

namespace Avtoobves.Models
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [MaxLength(256)]
        [Required(ErrorMessage = "Enter RU Title!")]
        public string Title { get; set; }
        
        [MaxLength(256)]
        [Required(ErrorMessage = "Enter UA Title!")]
        public string TitleUk { get; set; }
        
        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter RU Description!")]
        public string Description { get; set; }

        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter UA Description!")]
        public string DescriptionUk { get; set; }
        
        [MaxLength(256)]
        public string FirstImageUrl { get; set; }
        
        [MaxLength(256)]
        public string SecondImageUrl { get; set; }
        
        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter RU FirstParagraphText!")]
        public string FirstParagraphText { get; set; }

        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter UA FirstParagraphText!")]
        public string FirstParagraphTextUk { get; set; }
        
        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter RU SecondParagraphText!")]
        public string SecondParagraphText { get; set; }
        
        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter UA SecondParagraphText!")]
        public string SecondParagraphTextUk { get; set; }
        
        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter RU ThirdParagraphText!")]
        public string ThirdParagraphText { get; set; }

        [MaxLength(2048)]
        [Required(ErrorMessage = "Enter UA ThirdParagraphText!")]
        public string ThirdParagraphTextUk { get; set; }
    }
}