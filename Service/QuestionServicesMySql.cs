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

        public void CreateQuestion(string question, string[] wrongAnswers, string correctAnswer, int category_id)
        {
            string query = $"INSERT INTO question (question, correct_answer, category) VALUES(@question, @correctAnswer, @category_id); select last_insert_id();";
            string queryWrongAnswers = $"INSERT INTO wrong_answer (id_question, wrong_one, wrong_two, wrong_three, wrong_four) " +
                $"VALUES(@id_question, @wrong_one, @wrong_two, @wrong_three, @wrong_four); ";

            MySqlTransaction transaction = null;

            try
            {
                connection.Connect();
                transaction = connection.Connection.BeginTransaction();

                MySqlCommand cmd = connection.Connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.Connection = connection.Connection;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@question", question);
                cmd.Parameters.AddWithValue("@correctAnswer", correctAnswer);
                cmd.Parameters.AddWithValue("@category_id", category_id);
                int idQuestion = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = queryWrongAnswers;
                cmd.Parameters.AddWithValue("@id_question", idQuestion);
                cmd.Parameters.AddWithValue("@wrong_one", wrongAnswers[0]);
                cmd.Parameters.AddWithValue("@wrong_two", wrongAnswers[1]);
                cmd.Parameters.AddWithValue("@wrong_three", wrongAnswers[2]);
                cmd.Parameters.AddWithValue("@wrong_four", wrongAnswers[3]);
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    if(transaction != null) transaction.Rollback();
                }
                catch (MySqlException mySqlEx) 
                {
                    throw new Exception("Error during the insert transaction rollback.", mySqlEx);
                }
                throw new Exception("Fail in inerting new question. ", ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool DeleteQuestion(int id)
        {
            string queryDeleteWrongAnswers = $"DELETE FROM wrong_answer WHERE id_question = {id}";
            string queryDeleteQuestion = $"DELETE FROM question WHERE id = {id}";

            MySqlTransaction transaction = null;

            try
            {
                connection.Connect();
                transaction = connection.Connection.BeginTransaction();

                MySqlCommand cmd = new MySqlCommand(queryDeleteWrongAnswers, connection.Connection);
                cmd.ExecuteNonQuery(); 
                cmd.CommandText = queryDeleteQuestion;
                int rowsDeleted = cmd.ExecuteNonQuery();

                transaction.Commit();
                return rowsDeleted > 0;
            }
            catch (Exception ex)
            {
                try
                {
                    if (transaction != null) transaction.Rollback();
                }
                catch (MySqlException mySqlEx)
                {
                    throw new Exception("Error during the insert transaction rollback.", mySqlEx);
                }

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
                string query = "SELECT q.id, c.name as category, q.question, q.correct_answer, w.wrong_one, w.wrong_two, w.wrong_three, w.wrong_four "
                    + "FROM question q "
                    + "JOIN wrong_answer w ON q.id = w.id_question "
                    + "JOIN categories c on q.category = c.id;";

                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                List<QuestionDto> listQuestions = new List<QuestionDto>();
                while (reader.Read())
                {
                    listQuestions.Add(new QuestionDto
                    {
                        Id = reader.GetInt32("id"),
                        Question = reader.GetString("question"),
                        CorrectAnswer = reader.GetString("correct_answer"),
                        Category = reader.GetString("category"),
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

        public IEnumerable<CategoryDto> GetCategories()
        {
            string query = "SELECT id, name FROM categories;";
            try
            {
                List<CategoryDto> categories = new List<CategoryDto>();
                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new CategoryDto
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name")
                    });
                }
                return categories; 
            }
            catch (Exception ex)
            {
                throw new Exception("Fail maxQuestionsToRequest method in QuestionServicesMySql", ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public QuestionDto GetQuestion(int id)
        {
            string query = "SELECT q.id, c.name as category, q.question, q.correct_answer, w.wrong_one, w.wrong_two, w.wrong_three, w.wrong_four"
                    + "FROM question q"
                    + "JOIN wrong_answer w ON q.id = w.id_question"
                    + "JOIN categories c on q.category = c.id;"
                    + $"WHERE q.id = {id}";
            try
            {
                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                return new QuestionDto
                {
                    Id = reader.GetInt32("id"),
                    Question = reader.GetString("question"),
                    CorrectAnswer = reader.GetString("correct_answer"),
                    Category = reader.GetString("category"),
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
            //TODO
            throw new NotImplementedException();
        }


        public int MaxQuestionsToRequest()
        {
            string query = "select count(*) from question;";
            try
            {
                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                return int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Fail maxQuestionsToRequest method in QuestionServicesMySql", ex);
            }
            finally
            {
                CloseConnection();
            }
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
