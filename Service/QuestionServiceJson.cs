using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApplication2.Dto;

namespace WebApplication2.Service
{
    /// <summary>
    /// Deprecated, create only to practice with JSON files
    /// </summary>
    public class QuestionServiceJson : IQuestionService
    {
        private const string JSON_FILE_PATH = @"./Resources/QuestionsRepository.json";
        private readonly List<QuestionDto> questionsList = new List<QuestionDto>();

        public QuestionServiceJson()
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
            //  string dirPath = Directory.GetCurrentDirectory(); //heroku
            //  JArray questionsArray = JArray.Parse(File.ReadAllText(Path.Combine(dirPath,JSON_FILE_PATH )));//heroku
            JArray questionsArray = JArray.Parse(File.ReadAllText(JSON_FILE_PATH));

            foreach (JToken result in questionsArray.Children().ToList()) 
            {
                QuestionDto item = result.ToObject<QuestionDto>();
                questionItems.Add(item);
            }
            return questionItems;
        }

        public void DeleteQuestion(int id)
        {
            JArray questionsArray = JArray.Parse(File.ReadAllText(@".\Resources\QuestionsRepository.json"));
            //TODO
        }

        public void CreateQuestion(string question, string[] wrongAnswers, string correctAnswerd)
        {
            QuestionDto questionDto = new QuestionDto()
            {
                Id = maxQuestionsToRequest() + 1,
                Question = question,
                WrongAnswers = wrongAnswers,
                CorrectAnswer = correctAnswerd
            };

            JArray questionsArray = JArray.Parse(File.ReadAllText(@".\Resources\QuestionsRepository.json"));
            questionsArray.Add(JObject.FromObject(questionDto));

            try {
                File.WriteAllText(JSON_FILE_PATH, questionsArray.ToString());
            } catch(Exception ex) { throw ex; }
        }
    }
}
