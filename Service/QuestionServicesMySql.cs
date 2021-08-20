using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication2.Dto;
using WebApplication2.Service;

namespace SimpreRestApiQuestions.Service
{
    public class QuestionServicesMySql : IQuestionService
    {
        private readonly ConnectionDataBase connection;

        public QuestionServicesMySql()
        {
            connection = ConnectionDataBase.Instance();
        }

        public void CreateQuestion(string question, string[] wrongAnswers, string correctAnswerd)
        {
            throw new NotImplementedException();
        }

        public void DeleteQuestion(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QuestionDto> GetAllQuestions()
        {
            try
            {
                connection.Connect();

                string query = $"SELECT q.*, w.wrong_one, w.wrong_two, w.wrong_three, w.wrong_four FROM question q JOIN wrong_answer w ON q.id = w.id_question;";
                MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                var reader = cmd.ExecuteReader();

                List<QuestionDto> listQuestions = new List<QuestionDto>();
                while (reader.Read())
                {
                    listQuestions.Add(new QuestionDto
                    {
                        Id = reader.GetInt32("id"),
                        Question = reader.GetString("question"),
                        CorrectAnswer = reader.GetString("correct_answer"),
                        WrongAnswers = new string[]
                        {
                            reader.GetString("wrong_one"),
                            reader.GetString("wrong_two"),
                            reader.GetString("wrong_three"),
                            reader.GetString("wrong_four")
                        }
                    });
                }
                return listQuestions.AsEnumerable();
            }
            catch (Exception ex) 
            { 
                throw ex; 
            }
            finally 
            {
                CloseConnection();
            }
        }

        public QuestionDto GetQuestion(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QuestionDto> GetQuestions(int quantity)
        {
            throw new NotImplementedException();
        }

        public int maxQuestionsToRequest()
        {
            throw new NotImplementedException();
        }

        private void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
