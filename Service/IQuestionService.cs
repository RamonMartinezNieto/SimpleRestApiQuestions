using System.Collections.Generic;
using WebApplication2.Dto;

namespace WebApplication2.Service
{
    public interface IQuestionService
    {
        IEnumerable<QuestionDto> GetAllQuestions();

        IEnumerable<QuestionDto> GetQuestions(int quantity);

        QuestionDto GetQuestion(int id);

        bool DeleteQuestion(int id);

        void CreateQuestion(string question, string[] wrongAnswers, string correctAnswerd, int category_id);

        int MaxQuestionsToRequest();

        IEnumerable<CategoryDto> GetCategories();
    }
}
