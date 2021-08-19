using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApplication2.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly List<QuestionDto> questionsList = new List<QuestionDto>();

        public QuestionService()
        {
            lock (questionsList) { 
                questionsList = ReadJsonRepository();
            }
        }

        public int maxQuestionsToRequest() => questionsList.Count;

        public IEnumerable<QuestionDto> GetAllQuestions() => questionsList.AsEnumerable<QuestionDto>();

        public QuestionDto GetQuestion(int id) => questionsList.Where(q => q.Id == id).First();

        public IEnumerable<QuestionDto> GetQuestions(int quantity)
        {
            if (quantity > maxQuestionsToRequest()) 
            {
                throw new Exception($"No hay tntas preguntas, prguntas en el banco {maxQuestionsToRequest()}");
            }
            return questionsList.GetRange(0, quantity);
        }


        private List<QuestionDto> ReadJsonRepository() 
        {
            List<QuestionDto> questionItems = new List<QuestionDto>();
            string dirPath = Directory.GetCurrentDirectory(); //heroku
            JArray questionsArray = JArray.Parse(File.ReadAllText(Path.Combine(dirPath, @"/Resources/QuestionsRepository.json")));//heroku
            //JArray questionsArray = JArray.Parse(File.ReadAllText(@".\Resources\QuestionsRepository.json"));

            foreach (JToken result in questionsArray.Children().ToList()) 
            {
                QuestionDto item = result.ToObject<QuestionDto>();
                questionItems.Add(item);
            }
            return questionItems;
        }

    }
}
