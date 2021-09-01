using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleRestApiQuestions.Dto;
using SimpleRestApiQuestions.Service;
using SimpreRestApiQuestions.DataBase;

namespace SimpreRestApiQuestions.Service
{
    public class QuestionServicesMySql : IQuestionService
    {
        private readonly ConnectionDataBase connection;
        private const string SQL_SELECT_QUESTIONS = "SELECT q.id, c.name as category, q.question, q.correct_answer, w.wrong_one, w.wrong_two, w.wrong_three "
                    + "FROM question q "
                    + "JOIN wrong_answer w ON q.id = w.id_question "
                    + "JOIN categories c on q.category = c.id ";

        public QuestionServicesMySql()
        {
            connection = ConnectionDataBase.Instance();
        }

        public int CreateQuestion(string question, string[] wrongAnswers, string correctAnswer, int category_id)
        {
            string query = $"INSERT INTO question (question, correct_answer, category) VALUES(@question, @correctAnswer, @category_id); select last_insert_id();";
            string queryWrongAnswers = $"INSERT INTO wrong_answer (id_question, wrong_one, wrong_two, wrong_three) " +
                $"VALUES(@id_question, @wrong_one, @wrong_two, @wrong_three); ";
            string queryGetCurrentVersion = $"SELECT c._version as version FROM categories c where id = @category_id_version;";
            string queryUpdateVersion = $"UPDATE categories SET _version = @new_version WHERE id = @category_id_update;";

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
                cmd.ExecuteNonQuery();

                cmd.CommandText = queryGetCurrentVersion;
                cmd.Parameters.AddWithValue("@category_id_version", category_id);
                using MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int _currentVersion = reader.GetInt32("version");
                reader.Close(); 
                reader.Dispose(); 

                cmd.CommandText = queryUpdateVersion;
                cmd.Parameters.AddWithValue("@category_id_update", category_id);
                cmd.Parameters.AddWithValue("@new_version", (_currentVersion+1));
                cmd.ExecuteNonQuery();

                transaction.Commit();

                return idQuestion;
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
                int rowsDeleted = cmd.ExecuteNonQuery(); 
                cmd.CommandText = queryDeleteQuestion;
                int rowsDeletedTwo = cmd.ExecuteNonQuery();

                transaction.Commit();
                return rowsDeleted > 0 && rowsDeletedTwo > 0;
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

        public IEnumerable<QuestionDto> GetAllQuestions(int categoryId)
        {
            try
            {
                string query = SQL_SELECT_QUESTIONS + $" WHERE category = {categoryId};";

                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                return GetQuestionsFromReader(reader).AsEnumerable();

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
            //string query = "SELECT c.id, c.name, q.count(c.id) FROM categories c;";
            string query = "select c.id, c.name, count(*) as total_questions " +
                            "from question q " +
                            "join categories c on q.category = c.id " +
                            "group by q.category; ";

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
                        Name = reader.GetString("name"),
                        NumberOfQuestions = reader.GetInt32("total_questions")
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
            string query = SQL_SELECT_QUESTIONS + $"WHERE q.id = {id}";
            try
            {
                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                using MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return GetQuestionFromReader(reader);
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

        public IEnumerable<QuestionDto> GetQuestions(int quantity, int categoryId)
        {
            try
            {
                string query = SQL_SELECT_QUESTIONS + $" WHERE category = {categoryId} ORDER BY RAND() LIMIT {quantity}";
                connection.Connect();
                using MySqlCommand cmd = new MySqlCommand(query, connection.Connection);
                using MySqlDataReader reader = cmd.ExecuteReader();
                return GetQuestionsFromReader(reader).AsEnumerable();

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

        public int CreateCategory(string category)
        {
            string query = $"INSERT INTO categories (name, version) VALUES(@name_category, 0); select last_insert_id();";

            try
            {
                connection.Connect();

                MySqlCommand cmd = connection.Connection.CreateCommand();
                cmd.Connection = connection.Connection;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@name_category", category);
                return Convert.ToInt32(cmd.ExecuteScalar());
                
            }
            catch (Exception ex)
            {
                throw new Exception("Fail in inerting new question. ", ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool DeleteCategory(int id)
        {
            string queryDeleteCategory = $"DELETE FROM categories WHERE id = {id}";

            try
            {
                connection.Connect();

                MySqlCommand cmd = new MySqlCommand(queryDeleteCategory, connection.Connection);
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

        public int MaxQuestionsToRequest(int categoryId)
        {
            string query = $"select count(*) from question where category = {categoryId};";
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

        private List<QuestionDto> GetQuestionsFromReader(MySqlDataReader reader) 
        {
            try
            {
                List<QuestionDto> listQuestions = new List<QuestionDto>();
                while (reader.Read())
                {
                    listQuestions.Add(GetQuestionFromReader(reader));
                }
                return listQuestions;
            }
            catch (Exception ex) 
            {
                throw new Exception("Error getting questionDto from Reader", ex);
            }
        }

        private QuestionDto GetQuestionFromReader(MySqlDataReader reader) 
        {
            try
            {
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
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting questionDto from Reader", ex);
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
