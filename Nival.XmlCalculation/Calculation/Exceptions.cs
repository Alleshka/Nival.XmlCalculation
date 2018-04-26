using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nival.XmlCalculation.Calculation
{
    public class UnKnownOperandException : Exception
    {
        public UnKnownOperandException(String oper) : base($"Неизвестный операнд {oper}\nПоддерживаются следующие операнды: add, multiply, divide, subtract")
        {

        }
    }

    public class UnKnownAttributeException : Exception
    {
        public UnKnownAttributeException(String attribute) : base($"Неизвестный атрибут {attribute}")
        {

        }
    }
}
