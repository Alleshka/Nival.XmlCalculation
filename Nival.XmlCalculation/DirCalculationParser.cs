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
        private object locker = new object();
        private List<Calculations> calculationsList = new List<Calculations>();

        public DirCalculationParser(String directoryPath)
        {
            Queue<String> fileList = new Queue<string>(Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories)); // Получаем список файлов

            if (fileList.Count != 0)
            {
                Task[] taskArray = new Task[fileList.Count];

                while (fileList.Count != 0)
                {
                    String curFile = fileList.Dequeue();
                    taskArray[fileList.Count] = Task.Factory.StartNew(Action, curFile);
                    //Action(curFile);
                }

                Task.WaitAll(taskArray);
                var maxCalculations = calculationsList.OrderBy(x => x.Info.CalculateCount).Last().Info;
                Console.WriteLine("\n" + new String('-', 15) + "\n");
                Console.WriteLine($"Максимальное число элементов = {maxCalculations.CalculateCount} в файле = {maxCalculations.FileName}");

                Console.WriteLine("\n" + new String('-', 15));
                Console.WriteLine("Список ошибок: ");
                foreach(var calculation in calculationsList)
                {
                    if (calculation.GetSkippedInfo().Count() != 0)
                    {
                        Console.WriteLine($"Файл {calculation.Info.FileName}: \n");
                        Console.WriteLine(calculation.GetSkippedInfo());
                    }
                }
            }
        }

        private void Action(object curFile)
        {
            try
            {
                // Парсим файл и считаем
                Calculations calculations = new Calculations(curFile as String);
                calculationsList.Add(calculations);

                calculations.Calculate();
                Console.WriteLine(calculations.Info.ToString()); // Выводим информацию о подсчёе
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{curFile} - {ex.Message}");
            }
        }
    }
}