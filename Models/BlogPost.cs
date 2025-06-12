using System;
using System.ComponentModel.DataAnnotations;

namespace Avtoobves.Models
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        
        public string CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [Required(ErrorMessage = "Enter Title!")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Enter Description!")]
        public string Description { get; set; }
        
        public string FirstImageUrl { get; set; }
        
        public string SecondImageUrl { get; set; }
        
        [Required(ErrorMessage = "Enter FirstParagraphText!")]
        public string FirstParagraphText { get; set; }
        
        [Required(ErrorMessage = "Enter SecondParagraphText!")]
        public string SecondParagraphText { get; set; }
        
        [Required(ErrorMessage = "Enter ThirdParagraphText!")]
        public string ThirdParagraphText { get; set; }
    }
}