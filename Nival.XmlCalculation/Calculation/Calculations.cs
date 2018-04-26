using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XmlConfiguration;

namespace Nival.XmlCalculation.Calculation
{
    class CalculationInfo
    {
        public double Response { get; private set; }
        public double CalculateCount { get; private set; }
        public String FileName { get; private set; }

        public CalculationInfo(double response, double count, String file)
        {
            Response = response;
            CalculateCount = count;
            FileName = file;
        }

        public override string ToString()
        {
            return $"Файл = {FileName}: Результат = {Response}";
        }
    }
    class SkippedElements
    {
        public String ElemName { get; private set; }
        public String Reason { get; private set; }
        public String Fix { get; private set; }

        public SkippedElements(String elem, String msg, String fix = "")
        {
            ElemName = elem;
            Reason = msg;
            Fix = fix;
        }

        public override string ToString()
        {
            String response = $"Узел: {ElemName}\nОшибка: {Reason}";
            if (Fix.Length!=0) response += $"\nСпособ исправления: ({Fix})";
            return response + "\n\n";
        }
    }

    class Calculations
    {
        public CalculationInfo Info { get; private set; }

        private List<Calculation> calculationList;
        private List<SkippedElements> skippedElemens;

        private String fileName;

        public Calculations(String path)
        {
            fileName = path;
            calculationList = new List<Calculation>();
            skippedElemens = new List<SkippedElements>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path); // Считываем файл
           

            XmlNode calculationsNode = xmlDocument.DocumentElement; // Получаем корневой элемент calculations
            foreach(XmlNode node in calculationsNode)
            {
                Calculation calculation = ParseCalculationNode(node);
                if (calculation != null) calculationList.Add(calculation);
            }
        }

        public double Calculate()
        {
            double response = 0;

            foreach(var calculate in calculationList)
            {
                switch(calculate.Operand)
                {
                    case OperandType.ADD:
                        {
                            response += calculate.Mod;
                            break;
                        }
                    case OperandType.DIVIDE:
                        {
                            response /= calculate.Mod;
                            break;
                        }
                    case OperandType.MULTIPLY:
                        {
                            response *= calculate.Mod;
                            break;
                        }
                    case OperandType.SUBSTRACT:
                        {
                            response -= calculate.Mod;
                            break;
                        }
                     
                }            
            }

            Info = new CalculationInfo(response, calculationList.Count, fileName);
            return response;
        }

        public String GetSkippedInfo()
        {
            StringBuilder response = new StringBuilder();

            foreach(var item in skippedElemens)
            {
                response.Append(item.ToString());
            }

            return response.ToString();
        }

        private Calculation ParseCalculationNode(XmlNode calculationNode)
        {
            Calculation calculation = null;

            XmlNode uidAttribute = null;
            XmlNode operandAttribute = null;
            XmlNode modAttribute = null;

            foreach (XmlNode node in calculationNode) // Проходим по всем элементам calculation
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    // Парсим аттрибуты
                    String attributeName = node.Attributes.GetNamedItem("name").Value; // Достаём имя аттрибута

                    switch (attributeName)
                    {
                        case "uid":
                            {
                                uidAttribute = node.Attributes.GetNamedItem("value");
                                break;
                            }
                        case "operand":
                            {
                                operandAttribute = node.Attributes.GetNamedItem("value");
                                break;
                            }
                        case "mod":
                            {
                                modAttribute = node.Attributes.GetNamedItem("value");
                                break;
                            }
                        default:
                            {
                                // Ничего не делаем
                                break;
                            }
                    }
                }
            }

            if (uidAttribute == null)
            {
                skippedElemens.Add(new SkippedElements(calculationNode.OuterXml, "Отсутствует элемент uid", "Добавьте элемент uid (Пример: <str name=\"uid\" value=\"6ee78645316debb65ff35488b8822b2a\"/>)"));
                return null;
            }

            if(operandAttribute == null)
            {
                skippedElemens.Add(new SkippedElements(calculationNode.OuterXml, "Отсутствует элемент operand", "Добавьте элемент operand (Пример: <str name=\"operand\" value=\"divide\" />"));
                return null;
            }

            if (modAttribute == null)
            {
                skippedElemens.Add(new SkippedElements(calculationNode.OuterXml, "Отсутствует элемент mod", "Добавьте элемент mod (Пример: <int name=\"mod\" value=\"3\" />"));
                return null;
            }

            Guid uid = Guid.Empty;
            OperandType operand = OperandType.ADD;
            double mod = 0;
          
            try
            {
                uid = Guid.Parse(uidAttribute.Value);
            }
            catch(FormatException ex)
            {
                skippedElemens.Add(new SkippedElements(calculationNode.OuterXml, ex.Message));
                return null;
            }

            try
            {
                operand = ParseOperand(operandAttribute.Value);
            }
            catch (UnKnownOperandException ex)
            {
                skippedElemens.Add(new SkippedElements(calculationNode.OuterXml, ex.Message, $"Укажите один из приведённых операндов в аттрибут {operandAttribute.OuterXml}"));
                return null;
            }

            try
            {
                mod = Convert.ToDouble(modAttribute.Value);
            }
            catch(FormatException)
            {
                skippedElemens.Add(new SkippedElements(calculationNode.OuterXml, $"{modAttribute.Value} не является числом", $"Укажите число в аттрибут {modAttribute.OuterXml}"));
                return null;
            }


            calculation = new Calculation(uid, operand, mod);
            return calculation;
        }

        private OperandType ParseOperand(String operandString)
        {
            OperandType operandType = OperandType.ADD;
            switch(operandString)
            {
                case "add":
                    {
                        operandType = OperandType.ADD;
                        break;
                    }
                case "multiply":
                    {
                        operandType = OperandType.MULTIPLY;
                        break;
                    }
                case "divide":
                    {
                        operandType = OperandType.DIVIDE;
                        break;
                    }
                case "subtract":
                    {
                        operandType = OperandType.SUBSTRACT;
                        break;
                    }
                default:
                    {
                        throw new UnKnownOperandException(operandString);
                    }
            }
            return operandType;
        }

    }
}
