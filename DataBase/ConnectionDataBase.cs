using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpreRestApiQuestions
{
    public class ConnectionDataBase
    {
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string SslMode { get; set; }

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
