using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTrialTask
{
    /// <summary>
    /// Класс для работы с Postgre SQL
    /// </summary>
    static class JobPostgreSQL
    {
        
        static string connectionString = ConfigurationManager.ConnectionStrings["MyServPostgreSQL"].ConnectionString;
        static string nameLocalServerPogresSQL = ConfigurationManager.AppSettings.Get("LocalServer"); //значение value


        /// <summary>
        /// Получение размера БД
        /// </summary>
        public static string GetSizeTablesAsync()
        {
           string request = @"SELECT pg_size_pretty(pg_database_size(current_database()));";
           string result = null;
          
            try
            {
                //Соединение к серверу
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                   
                NpgsqlCommand execute_command = new NpgsqlCommand(request, connection);
               
                result += execute_command.ExecuteScalar().ToString(); //Выполняем нашу команду.
                }
            }
            catch (Exception ex)
            {
                result += DateTime.Now.ToString() + $": Ошибка при подключении к серваку {ex.ToString()}" ;
               
            }

            finally
            {
                //connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Получение списка таблиц из БД
        /// </summary>
       public static void GetListTables()
        {
            string request = @"SELECT table_name FROM information_schema.tables
            //                  WHERE table_schema NOT IN('information_schema', 'pg_catalog')
            //                  AND table_schema IN('public', 'myschema') ";
        }


        /// <summary>
        /// Чтение файла настроек web.config. Для подключение к Бд 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetServersList()
        {
            List<string> serverList = new List<string>();

            for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                if (ConfigurationManager.ConnectionStrings[i].ProviderName == "Npgsql")
                    serverList.Add(ConfigurationManager.ConnectionStrings[i].Name);
            }

            return serverList;
        }


        /// <summary>
        /// Метод для соединения(запроса) к бд  и возрату листа с данными из БД
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTable()
        {
           // List<StringBuilder> table = new List<StringBuilder>();
           // List<string> table = new List<string>();
            // StringBuilder strimTemp = null;
            List<string> strimTemp = new List<string>(); 

            //Временная таблица бд
            DataTable test_table = new DataTable();

            string sql_request = @"
SELECT pg_database.datname 
AS NameBD,
pg_database_size(pg_database.datname)
AS size_DB                                    
FROM pg_database
WHERE pg_database.datname = 'postgres' 
";


            //соединение с бд
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand execute_command = new NpgsqlCommand(sql_request, connection);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                string temp = $" * *Ошибка при сединении с сервером! Server: {connectionString}. {Environment.NewLine} Описание ошибки :{Environment.NewLine} {ex.ToString()}**";
                strimTemp.Add(temp);
               ;
            }
            NpgsqlDataReader reader;

            //Получаем результат запроса к БД
            reader = execute_command.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    strimTemp.Add($"Имя сервера: {reader.GetValue(0)}{Environment.NewLine}");
                    strimTemp.Add($"Размер БД: {reader.GetValue(1).ToString()}{Environment.NewLine}");
                   // strimTemp.Add($"Свободной памяти {reader.GetString(0)}{Environment.NewLine}");
                    strimTemp.Add($"{DateTime.Now.ToString("dd.MM.yyyy")}{Environment.NewLine}");
                    strimTemp.Add($"Размер дисков в конфиге: {nameLocalServerPogresSQL}{Environment.NewLine}");
                   // strimTemp.Append($"{Environment.NewLine}");
                   
                }
                catch (Exception ex)
                {
                    strimTemp.Add($"{DateTime.Now.ToString()}  :Ошибка при получении информации о БД: {ex}");
                    
                }
            }
            connection.Close();
            return strimTemp;
        }

        public static string GetTable2()
        {
            List<IList<Object>> table = new List<IList<Object>>();
           

            string strimTemp = null; 

            //Временная таблица бд
            DataTable test_table = new DataTable();

            //запрос к sql
            //string sql_request = @"SELECT pg_database.datname
            //                            AS database_name,
            //                        pg_database_size(pg_database.datname)
            //                            AS database_size_bytes                                    
            //                FROM pg_database
            //                UNION ALL
            //                SELECT 'Свободно'
            //                            AS database_name,
            //                        sum(pg_database_size(pg_database.datname))
            //                            AS database_size_bytes
            //                FROM pg_database;";

            string sql_request = @"
                         SELECT pg_database.datname 
AS NameBD,
pg_database_size(pg_database.datname)
AS size_DB                                    
FROM pg_database
WHERE pg_database.datname = 'postgres' 
UNION ALL
SELECT 'Свободно'
AS database_name,
sum(pg_database_size(pg_database.datname))
AS database_size_bytes
FROM pg_database
                                    ";
            //соединение с бд
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand execute_command = new NpgsqlCommand(sql_request, connection);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
              //  string temp = $"**Ошибка при сединении с сервером! Server: {connectionString}. {Environment.NewLine} Описание ошибки :{Environment.NewLine} {ex.ToString()}**";
                //strimTemp.Append(temp);
                //table.Add(strimTemp);
            }
            NpgsqlDataReader reader;

            //Получаем результат запроса к БД
            reader = execute_command.ExecuteReader();

           int temp = reader.FieldCount;
           int jj = reader.VisibleFieldCount;
           // strimTemp = new string[2, 2];
           //int i = 0;

            while (reader.Read())
            {
                try
                {
                    //strimTemp = new string[temp, temp];
                    //  strimTemp = new IList<Object>();

                    for (int i = 0; i < 10; i++)
                    {
                        //for (int j = 0; j < 5+1; j++)
                        //{
                        // strimTemp[i, 0] = $"Имя сервера: {reader.GetValue(0)}{Environment.NewLine}";
                        //// /*strimTemp[0, 0]*/ += $"Имя сервера: {reader.GetValue(0)}{Environment.NewLine}"
                        // strimTemp[i,1 ] = $"Размер БД: {reader.GetValue(1).ToString()}{Environment.NewLine}";
                        // strimTemp[i,2] = $"Свободной памяти {reader.GetString(0)}{Environment.NewLine}";
                        // strimTemp[i,4] = $"{DateTime.Now.ToString("dd.MM.yyyy")}{Environment.NewLine}";
                        // strimTemp[i,3] = $"Размер дисков в конфиге: {nameLocalServerPogresSQL}{Environment.NewLine}";

                         strimTemp += $"Имя сервера: {reader.GetValue(0)}{Environment.NewLine}";
                       // strimTemp += $"Имя сервера: {reader.GetValue(0)}{Environment.NewLine}";
                        strimTemp += $"Размер БД: {reader.GetValue(1).ToString()}{Environment.NewLine}";
                        // strimTemp[i,2] = $"Свободной памяти {reader.GetString(0)}{Environment.NewLine}";
                        strimTemp += $"{DateTime.Now.ToString("dd.MM.yyyy")}{Environment.NewLine}";
                        // strimTemp[i,3] = $"Размер дисков в конфиге: {nameLocalServerPogresSQL}{Environment.NewLine}";
                        strimTemp += $"*";

                    }


                    //strimTemp[0,0] += $"Имя сервера: {reader.GetValue(0)}{Environment.NewLine}";
                    //strimTemp[0, 1] = $"Размер БД: {reader.GetValue(1).ToString()}{Environment.NewLine}";
                    //strimTemp[0, 2] = $"Свободной памяти {reader.GetString(0)}{Environment.NewLine}";
                    ////strimTemp.Append($"{DateTime.Now.ToString("dd.MM.yyyy")}{Environment.NewLine}");
                    //strimTemp[0, 3] = $"Размер дисков в конфиге: {nameLocalServerPogresSQL}{Environment.NewLine}";
                    //strimTemp.Append($"{Environment.NewLine}");
                    //table.Add(strimTemp);
                }
                catch (Exception ex)
                {
                    // strimTemp.Append($"{DateTime.Now.ToString()}  :Ошибка при получении информации о БД: {ex}");
                    //table.Add(strimTemp);
                }
            }
            connection.Close();
            return strimTemp;
        }

    }

}

