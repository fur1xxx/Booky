using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Booky.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]    
        [DisplayName("Category name")]
        public string Name { get; set; }

        [DisplayName("Display order")]
        [Range(1,int.MaxValue, ErrorMessage = "Display order must be between 1-100")]
        public int DisplayOrder { get; set; }

        [ValidateNever]
        public ICollection<Product> Products { get; set; }
    }
}
