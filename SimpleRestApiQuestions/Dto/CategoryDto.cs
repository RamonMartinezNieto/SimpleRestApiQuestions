using System.ComponentModel.DataAnnotations;

namespace SimpleRestApiQuestions.Dto
{
    public class CategoryDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public int NumberOfQuestions { get; set; }

        public int NumberOfVersion { get; set; }
    }
}