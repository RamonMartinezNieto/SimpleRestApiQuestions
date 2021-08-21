using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dto
{
    public class QuestionDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public string[] WrongAnswers { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }
    }
}