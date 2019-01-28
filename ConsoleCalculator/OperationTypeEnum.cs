using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    internal enum OperationTypeEnum
    {
        Addition = '+',
        Subtraction = '-',
        Multiplication = '*',
        Division = '/',
        Exponentiation = '^'
    }

    static internal class OperationTypeEnumExtensions
    {
        public static bool IsPriorityEqualOrHigher(this OperationTypeEnum current, OperationTypeEnum other)
        {
            if (current == other)
            {
                return true;
            }
            else
            {
                switch (current)
                {
                    case OperationTypeEnum.Addition:
                        return other == OperationTypeEnum.Subtraction;
                    case OperationTypeEnum.Subtraction:
                        return other == OperationTypeEnum.Addition;
                    case OperationTypeEnum.Multiplication:
                        return other==OperationTypeEnum.Division || other == OperationTypeEnum.Addition || other == OperationTypeEnum.Subtraction;
                    case OperationTypeEnum.Division:
                         return other == OperationTypeEnum.Multiplication || other == OperationTypeEnum.Addition || other == OperationTypeEnum.Subtraction;
                    case OperationTypeEnum.Exponentiation:
                        return true;
                    default:
                        return false;
                }
            }
           
        }
            

    }
}
