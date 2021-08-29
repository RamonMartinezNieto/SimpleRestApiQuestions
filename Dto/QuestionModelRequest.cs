using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SimpleRestApiQuestions.Dto
{
    public class QuestionModelRequest
    {
        [Required]
        public string Question { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(3)]
        public string[] WrongAnswers { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        [Required]
        public int IdCategory { get; set; }
    }
}
