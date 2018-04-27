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
                DirCalculationParser parser = new DirCalculationParser(args[0]);
                parser.Start();
            }
            else
            {
                Console.WriteLine("Отсутствует путь до папки");
            }
        }
    }
}
