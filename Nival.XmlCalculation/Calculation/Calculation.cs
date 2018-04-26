using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Nival.XmlCalculation.Calculation
{
    public enum OperandType { ADD, MULTIPLY, DIVIDE, SUBSTRACT }

    class Calculation
    {
        public Guid UID { get; private set; }
        public OperandType Operand { get; private set; }
        public double Mod { get; private set; }

        public Calculation(Guid uid, OperandType type, double mod)
        {
            UID = uid;
            Operand = type;
            Mod = mod;
        }
    }
}