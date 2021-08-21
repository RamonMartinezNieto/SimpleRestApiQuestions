namespace WebApplication2.Dto
{
    public class QuestionDto
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string Question { get; set; }

        public string[] WrongAnswers { get; set; }

        public string CorrectAnswer { get; set; }
    }
}