using System.ComponentModel.DataAnnotations;

namespace SimpleRestApiQuestions.Dto
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
        [MinLength(3)]
        [MaxLength(3)]
        public string[] WrongAnswers { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }
    }
}