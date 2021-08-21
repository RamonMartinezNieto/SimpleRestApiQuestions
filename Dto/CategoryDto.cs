using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dto
{
    public class CategoryDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}