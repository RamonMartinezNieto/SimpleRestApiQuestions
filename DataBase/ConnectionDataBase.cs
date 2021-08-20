using MySql.Data.MySqlClient;
using System;

namespace SimpreRestApiQuestions
{
    public class ConnectionDataBase
    {
        private string Server { get; set; }
        private string DatabaseName { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private string Port { get; set; }
        private string SslMode { get; set; }

        public MySqlConnection Connection { get; private set; }

        //Singleton
        private static ConnectionDataBase _instance = null;
        public static ConnectionDataBase Instance()
        {
            if (_instance == null)
                _instance = new ConnectionDataBase();
            return _instance;
        }

        private ConnectionDataBase()
        {
            DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
            Server = Environment.GetEnvironmentVariable("DATABASE_HOST");
            UserName = Environment.GetEnvironmentVariable("DATABASE_USER");
            Password = Environment.GetEnvironmentVariable("DATABASE_PASS");
            Port = "3306";
            SslMode = "none";
        }

        public void Connect()
        {
            try {
                string connstring = String.Format($"server={Server};port={Port};user id={UserName}; password={Password}; database={DatabaseName}; SslMode={SslMode}");
                Connection = new MySqlConnection(connstring);
                Connection.Open(); 
            }
            catch (Exception ex) { throw new Exception("Error connecting with the data base", ex); }
        }   

        public void Close()
        {
            try { Connection.Close(); }
            catch (Exception ex) { throw new Exception("Error closing the data base", ex); }
        }
    }
}
