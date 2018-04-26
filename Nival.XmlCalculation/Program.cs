using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nival.XmlCalculation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                try
                {
                    DirCalculationParser parser = new DirCalculationParser(args[0]);
                }
                catch(System.IO.DirectoryNotFoundException)
                {
                    Console.WriteLine($"Не удалось найти папку {args[0]}");
                }
                catch(System.IO.IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else Console.WriteLine("Отсутствует путь до папки");
        }
    }
}
