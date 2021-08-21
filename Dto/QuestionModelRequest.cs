using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dto
{
    public class QuestionModelRequest
    {
        [Required]
        public string Question { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public string[] WrongAnswers { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        [Required]
        public int IdCategory { get; set; }
    }
}
