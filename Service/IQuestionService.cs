using System.Collections.Generic;
using SimpleRestApiQuestions.Dto;

namespace SimpleRestApiQuestions.Service
{
    public interface IQuestionService
    {
        IEnumerable<QuestionDto> GetAllQuestions(int categoryId);

        IEnumerable<QuestionDto> GetQuestions(int quantity, int categoryId);

        QuestionDto GetQuestion(int id);

        bool DeleteQuestion(int id);

        int CreateQuestion(string question, string[] wrongAnswers, string correctAnswerd, int category_id);

        int MaxQuestionsToRequest(int categoryId);

        IEnumerable<CategoryDto> GetCategories();

        int CreateCategory(string category);

        bool DeleteCategory(int id);
    }
}
