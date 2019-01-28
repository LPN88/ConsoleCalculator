using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("UnitTestCalculator")]

namespace ConsoleCalculator
{   
    internal static class Program
    {
        //Стек для хранения выражений в скобках
        internal static Stack<string> _expressions = null;

        //Список чисел в одном выражении
        internal static List<decimal> _decimals = null;

        //Список операций в одном выражении
        internal static List<OperationTypeEnum> _operations = null;

        internal static void Main(string[] args)
        {           
            string input = null;
            string error;
            bool repeat=false;           
            do
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Введите уравнение без пробелов:");
                    input = Console.ReadLine().Trim(); //"11+7+9/3*2-10*2+25+17**(19-3*(4+5*1.5)-1)";
                }
                else
                {
                    Console.WriteLine("Вы ввели уравнение: " + args[0]);
                    input = args[0].Trim();
                }
                try
                {
                    if (ValidateInput(input, out error))
                    {

                        var result = ComputeEquation(input.Replace(".", ","));
                        Console.WriteLine("Результат: " + result);
                    }
                    else
                    {
                        Console.WriteLine(error);
                    }
                }
                catch (DivideByZeroException e)
                {
                    Console.WriteLine("В процессе расчета произошла операция деления на 0 !");
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("В процессе расчета произошла ошибка !");
                }
                finally
                {
                    
                    args = new string[0];
                }
                Console.WriteLine("Продолжить ввод уравнения ? (y/n)");
                var ch = Console.ReadLine();
                repeat = ch[0].Equals('y') ? true : false;
            }
            while (repeat);
                       
        }

        /// <summary>
        /// Метод выполняет простую арифм. операцию и возвращает результат    
        /// </summary>
        internal static decimal CalcOperation(decimal first, decimal second, OperationTypeEnum type)
        {
            switch (type)
            {
                case OperationTypeEnum.Addition:
                    return first+second;
                case OperationTypeEnum.Subtraction:
                    return first-second;
                case OperationTypeEnum.Multiplication:
                    return first*second;
                case OperationTypeEnum.Division:
                    return first/second;
                case OperationTypeEnum.Exponentiation:
                    return (decimal)Math.Pow((double)first, (double)second);
                default:
                    return 0;
            }
        }

        /// <summary>
        ///  Метод выполняет набор арифм. операций, выполняя операции слева направо с учетом приоритета, и возвращает результат
        /// </summary>
        /// <param name="operations">Список оперций</param>
        /// <param name="decimals">Список десятичных чисел, участвующих в операции</param>      
        internal static decimal ComputeOperations(List<OperationTypeEnum> operations, List<decimal> decimals)
        {
            decimal newValue = 0;
            if (decimals.Count == 1)
            {
                newValue = decimals[0];
                decimals.Clear();
                return newValue;
            }            
            for (int i = 0; i < operations.Count-1; i++)
            {
                if (operations[i].IsPriorityEqualOrHigher(operations[i+1]))
                {                    
                    newValue = CalcOperation(decimals[i], decimals[i+1], operations[i]);
                    decimals.RemoveRange(i, 2);
                    decimals.Insert(i, newValue);
                    operations.RemoveAt(i);
                    --i;
                }
                else
                {
                    newValue = CalcOperation(decimals[i+1], decimals[i + 2], operations[i+1]);
                    decimals.RemoveRange(i+1, 2);
                    decimals.Insert(i+1, newValue);
                    operations.RemoveAt(i+1);
                    --i;
                }
            }
            newValue = CalcOperation(decimals[0], decimals[1], operations[0]);
            decimals.Clear();
            operations.Clear();
            return newValue;
        }

        /// <summary>
        ///  Подготовка и расчет выражения внутри одних скобок путем парсинга операций и чисел в отдельные списки
        /// </summary>
        /// <param name="expr">выражение внутри скобок</param>
        /// <returns></returns>
        internal static decimal ComputeExpression(string expr)
        {
            int startIndex = 0;
            OperationTypeEnum currentOp;
            bool isPrevOp = false;
            for (int i = 0; i < expr.Length; i++)
            {                
                if (Enum.GetValues(typeof(OperationTypeEnum)).Cast<int>().Select(c => Convert.ToChar(c)).Any(c=> c.Equals(expr[i])))
                {
                    currentOp = (OperationTypeEnum)expr[i];
                    if (i == 0 || isPrevOp)
                    {
                        continue;
                    }
                    _operations.Add(currentOp);
                    _decimals.Add(decimal.Parse(expr.Substring(startIndex, i - startIndex), CultureInfo.GetCultureInfo("ru-Ru")));                  
                    startIndex = i + 1;
                    isPrevOp = true;
                }
                else
                {
                    isPrevOp = false;
                }
            }
            _decimals.Add(decimal.Parse(expr.Substring(startIndex, expr.Length - startIndex), CultureInfo.GetCultureInfo("ru-Ru")));
             return ComputeOperations(_operations, _decimals);         
        }

        /// <summary>
        /// Расчет уравнения
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static decimal ComputeEquation(string input)
        {
            _expressions = new Stack<string>();
            _decimals = new List<decimal>();
            _operations = new List<OperationTypeEnum>();
            StringBuilder sb = new StringBuilder(string.Empty);
            string calcValue = null;          
            try
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '(')
                    {                       
                            _expressions.Push(sb.ToString());
                            sb.Clear();                     
                    }
                    else if (input[i] == ')')
                    {
                        if (sb.Length > 0)
                        {
                           
                            if (_expressions.Count() > 0)
                            {
                                calcValue = ComputeExpression(sb.ToString()).ToString();
                                sb = new StringBuilder(_expressions.Pop() + calcValue);
                            }
                            //else
                            //{
                            //    sb = new StringBuilder (calcValue);
                            //}                            
                        }                      
                    }
                    else
                    {
                        sb.Append(input[i]);
                    }
                }
                return ComputeExpression(sb.ToString());
            }
            catch (Exception e)
            {
                _expressions.Clear();
                _operations.Clear();
                _decimals.Clear();
                throw e;
            }
                 
        }

        /// <summary>
        /// Валидация введенного уравнения
        /// </summary>
        /// <param name="input"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        internal static bool ValidateInput(string input, out string error)
        {
            var correctSymbols = new[] { '+', '-', '/', '*',',','.','^' };
            var correctAll = (new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0','(',')' }).Union(correctSymbols);
            if (string.IsNullOrEmpty(input))
            {
                error = "Вы не ввели арифметического выражения !";
                return false;
            }
            else if (input.Contains(" "))
            {
                error = "Вы ввели пробел внутри арифметического выражения !";
                return false;
            }
            else if(input.Select(c => c == '(').Count() != input.Select(c => c == ')').Count())
            {
                error = "Кол-во открывающих скобок не соответствует числу закрывающих !";
                return false;
            }
            else if(input.Split(correctSymbols).Where(s => s == string.Empty).Count() > 0 || input.Where(c => !correctAll.Contains(c)).Count() > 0)
            {
                error = "Вы ввели некорректное арифметическое выражение !";
                return false;
            }
            else
            {
                error = null;
                return true;
            }
        }
    }
}