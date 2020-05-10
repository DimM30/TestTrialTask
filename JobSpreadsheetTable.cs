using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTrialTask
{
    static class JobSpreadsheetTable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly string ClientSecret = "client_secret.json";

        /// <summary>
        /// Масив прав для работы с API
        /// </summary>
        private static readonly string[] ScopesShets = { SheetsService.Scope.Spreadsheets };

        /// <summary>
        /// Имя файла api гугла
        /// </summary>
        private static readonly string AppName = "fileGoogle.json";

        /// <summary>
        /// Индификтор таблицы 
        /// </summary>
        private static readonly string SpreadsheetId = "1nr9EpJ9CEezikivzWVIGpr7aooOvWuZN6MsQlio-3_g";

        /// <summary>
        /// Диапозон для получения с таблицы(колонка)
        /// </summary>
        const string Range = "A1:A4";

        public static string result = null;

        static List<string> dataDb = JobPostgreSQL.GetTable();
 
        ////Данные которые нужно добавть в таблицу c помощью двухмерного массива
        private static string[,] Data;

       

        /// <summary>
        /// Тестовой инициализатор записи в гугл таблицу
        /// </summary> 
        private static async Task testAsync()
        {
           // await Task.Delay();
            //Получаем учетные данные
            UserCredential credential = GetSheetCredential();
           
            //Получаем сервис
            SheetsService servise = (SheetsService)GetServise(credential);
            #region Стиль кода 'Вырви глаз'
            Data = new string[1, 8];
           
            #region ОПАСТНО!!! НЕ смотреть. Страшный 'говно'Кот)))
            for (int i = 0; i < 20; i++)
            {
                int x = 0;
               
                foreach (var item in dataDb)
                    {
                 
                 Data[i, x] = item;
                
                    if (x == 7)
                    {
                        return;
                    }
                    else if (x==3)
                    {
                        FillSpreadsheet(servise, SpreadsheetId, Data);

                        return;
                    }
                 x++;

                //}
              }

            }
            #endregion

            #endregion
        
        }


        /// <summary>
        /// Создание новой таблицы
        /// </summary>
        /// <param name="service"></param>
        /// <param name="sheetName"></param>
        /// <param name="SpreadsheetId"></param>
        public static void CreateNewSheet(SheetsService service, string sheetName, string SpreadsheetId)
        {
            var addShettRequest = new Request
            {
                //Добавляет новый лист.
                AddSheet = new AddSheetRequest
                {
                    // Свойства листа.
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                    
                }
            };

            List<Request> requests = new List<Request> { addShettRequest };

            //обновление любого аспекта электронной таблицы
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheet = new BatchUpdateSpreadsheetRequest();
            //Список обновлений, которые необходимо применить к электронной таблице. 
            batchUpdateSpreadsheet.Requests = requests;

            //Применяет одно или несколько обновлений к электронной таблице. Каждый запрос проверяется заранее применяется. 
            //Если какой-либо запрос не является действительным, то весь запрос не будет выполнен.
            service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheet, SpreadsheetId).Execute();
        }
        
        /// <summary>
        /// Метод инициализации 
        /// </summary>
        public static async void InitStart()
        {
            await Task.Run(() => testAsync());
          //  await Task.Delay(6005);
            #region Тестовые методы
            ////Получаем учетные данные
            // UserCredential credential = GetSheetCredential();

            ////Получаем сервис
            // SheetsService servise = (SheetsService)GetServise(credential);

            //заполнить данные и записать в гугл таблицу
            // FillSpreadsheet(servise, SpreadsheetId, Data);

            //При необходимости создаем новую таблицу
            // CreateNewSheet(servise, "Бэээээ", SpreadsheetId);

            //Получаем данные с заполненной таблице
            // result = GetFirstCell(servise, Range, SpreadsheetId);
            #endregion

        }

        /// <summary>
        /// Получаем учетные данные для работы с API таблиц
        /// </summary>
        /// <returns></returns>
        private static UserCredential GetSheetCredential()
        {

            using (var stream = new FileStream(ClientSecret, FileMode.Open, FileAccess.Read))
            {
                //Записываем путь к файлу и имя файла
                var credPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetsCreds.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        ScopesShets,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;

            }

 
        }

        /// <summary>
        /// Метод получает экземпяр сервиса для работы с таблицой 
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        private static object GetServise(UserCredential credential)
        {
            return new SheetsService(new BaseClientService.Initializer  
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName


            });
            
        }

        /// <summary>
        /// Метод заполнения таблицы
        /// </summary>
        /// <param name="servise"></param>
        /// <param name="SpreadsheetId"></param>
        /// <param name="data"></param>
        private static void FillSpreadsheet(SheetsService servise, string SpreadsheetId, string[,] data)
        {
            //лист для хранения запросов заполнение строки
            List<Request> requests = new List<Request>();

            //в цике проходим по  строке 2ного массива и записыам оттуда нужные данные.
            for (int i = 0; i<data.GetLength(0); i++)
            {
                
                List<CellData> values = new List<CellData>();
              
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    //Значения, которые может иметь ячейка в электронной таблице.
                    values.Add( new CellData  
                        {
                       UserEnteredValue = new ExtendedValue
                       {
                           // строковое значение.
                           StringValue = data[i,j]
                       }
                        });
                }

                requests.Add(
                    new Request
                    {
                        //Координата на листе.
                        UpdateCells = new UpdateCellsRequest
                        {
                            Start = new GridCoordinate
                            {
                                SheetId = 0,
                                RowIndex = 1,
                                ColumnIndex =0
                            },   //Значения в строке, по одному на столбец.
                            Rows = new List<RowData> {new RowData {Values = values } },
                            Fields = "userEnteredValue"
                        }
                    }
                    );
            }

            BatchUpdateSpreadsheetRequest burs = new BatchUpdateSpreadsheetRequest
            {
                Requests = requests
            };

            //Выполнение обновления электронной таблице
            servise.Spreadsheets.BatchUpdate(burs, SpreadsheetId).Execute();


        }


        /// <summary>
        /// Получения данных со столбца
        /// </summary>
        /// <param name="servise"></param>
        /// <param name="range"></param>
        /// <param name="spreadsheetId"></param>
        /// <returns></returns>
        private static string GetFirstCell(SheetsService servise, string range, string spreadsheetId)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = servise.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();

            string result = null;

            foreach (var value in response.Values)
            {
                result += " " + value[1];
            }
            return result;
        }
    }
}
