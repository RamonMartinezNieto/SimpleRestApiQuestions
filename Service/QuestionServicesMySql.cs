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

        public bool DeleteQuestion(int id)
        {
            string query = $"DELETE FROM question WHERE id = {id}";
            try
            {
                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                int rowsDeleted = cmd.ExecuteNonQuery();
                return rowsDeleted > 0;
            }
            catch (Exception ex) 
            {
                throw new Exception("Fail in DeleteQuestion class QuestionServiceMySql. ", ex);
            }
            finally 
            {
                CloseConnection();
            }
        }

        public IEnumerable<QuestionDto> GetAllQuestions()
        {
            try
            {
                string query = $"SELECT q.*, w.wrong_one, w.wrong_two, w.wrong_three, w.wrong_four FROM question q JOIN wrong_answer w ON q.id = w.id_question;";
                connection.Connect();
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
                throw new Exception("Fail in GetAllQuestions class QuestionServiceMySql. ", ex); 
            }
            finally 
            {
                CloseConnection();
            }
        }

        public QuestionDto GetQuestion(int id)
        {
            string query = $"SELECT q.*, w.wrong_one, w.wrong_two, w.wrong_three, w.wrong_four FROM question q JOIN wrong_answer w ON q.id = w.id_question WHERE q.id = {id};";
            try
            {
                connection.Connect();
                MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                var reader = cmd.ExecuteReader();

                return new QuestionDto
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
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Fail GetQuestion(int id) in QuestionServicesMySql method", ex);
            }
            finally 
            {
                CloseConnection();
            }
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
