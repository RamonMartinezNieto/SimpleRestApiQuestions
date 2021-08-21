namespace WebApplication2.Dto
{
    public class QuestionModelRequest
    {
        public string Question { get; set; }

        public string[] WrongAnswers { get; set; }

        public string CorrectAnswer { get; set; }

        public int IdCategory { get; set; }
    }
}
