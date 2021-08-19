using System.Collections.Generic;

namespace WebApplication2.Service
{
    public interface IQuestionService
    {
        IEnumerable<QuestionDto> GetAllQuestions();

        IEnumerable<QuestionDto> GetQuestions(int quantity);

        QuestionDto GetQuestion(int id);

        int maxQuestionsToRequest();

    }
}
