using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public static class MathUtils
    {
        public static double Compute(double left, double right, Operator op)
        {
            switch (op)
            {
                case Operator.Multiply:
                    return left * right;
                case Operator.Divide:
                    return left / right;
                case Operator.Add:
                    return left + right;
                case Operator.Subtract:
                    return left - right;
            }
            throw new InvalidOperationException();
        }
    }
}
