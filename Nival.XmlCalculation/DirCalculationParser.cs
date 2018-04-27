using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Nival.XmlCalculation.Calculation;
using System.Threading;


namespace Nival.XmlCalculation
{
    public class DirCalculationParser
    {
        /// <summary>
        /// Делегат вывода сообщений
        /// </summary>
        /// <param name="message">Сообщение</param>
        public delegate void PrintDelegate(String message);
        public PrintDelegate PrinAction;

        // Список файлов
        private List<Calculations> calculationsList; // Объекты подсчёта
        private Queue<String> fileList; // Файлы, которые необходимо распарсить

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        private String delimiter = new string('=', 32);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath">Путь до папки</param>
        /// <param name="print">Метод для вывода сообщений</param>
        public DirCalculationParser(String directoryPath, PrintDelegate print = null)
        {
            stopwatch.Start();

            // Назначаем метод вывода
            if (print != null)
            {
                PrinAction = print;
            }
            else
            {
                PrinAction = (x) => Console.WriteLine(x);
            }

            // Считываем списк файлов
            try
            {
                 fileList = new Queue<string>(Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories)); // Получаем список файлов
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                PrinAction?.Invoke($"Не удалось найти папку {directoryPath}");
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            calculationsList = new List<Calculations>();
        }

        public void Start()
        {
            if (fileList != null && fileList.Count != 0)
            {
                Task[] taskArray = new Task[fileList.Count];

                // Запускаем парсинг файлов
                while (fileList.Count != 0)
                {
                    String curFile = fileList.Dequeue();
                    taskArray[fileList.Count] = Task.Factory.StartNew(Action, curFile);
                    //Action(curFile);
                }
                Task.WaitAll(taskArray);

                // Выводим список ошибок
                if (!calculationsList.All(x => x.GetSkippedCount() == 0))
                {
                    PrinAction?.Invoke("\n" + delimiter);
                    PrinAction?.Invoke($"\nСписок ошибок: {calculationsList.Sum(x=>x.GetSkippedCount())} \n");
                    foreach (var calculation in calculationsList)
                    {
                        if (calculation.GetSkippedCount() != 0)
                        {
                            PrinAction?.Invoke(calculation.GetSkippedDesription());
                        }
                    }
                }
                else PrinAction?.Invoke("Ошибок нет");

                // Находим файл с максимальным количеством элементов
                var maxCalculations = calculationsList.OrderBy(x => x.Info.CalculateCount).Last().Info;
                PrinAction?.Invoke("\n" + delimiter + "\n");
                PrinAction?.Invoke($"Максимальное число элементов = {maxCalculations.CalculateCount} в файле = {maxCalculations.FileName}");
                PrinAction?.Invoke($"Время выполнения программы: {stopwatch.Elapsed.Milliseconds} мc. \n");
            }
            else PrinAction?.Invoke($"Файлы xml не найдены");
            stopwatch.Stop();
        }

        private void Action(object curFile)
        {
            try
            {
                // Парсим файл и считаем
                Calculations calculations = new Calculations(curFile as String);
                calculationsList.Add(calculations);

                calculations.Calculate();
                PrinAction?.Invoke(calculations.Info.ToString()); // Выводим информацию о подсчёте
            }
            catch(Exception ex)
            {
                PrinAction?.Invoke($"{curFile} - {ex.Message}");
            }
        }
    }
}