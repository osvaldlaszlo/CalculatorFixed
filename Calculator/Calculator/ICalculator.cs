using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public interface ICloneable<out T>
    {
        T Clone();
    }

    public abstract class Operation : ICloneable<Operation>//make an implicit operator (google implicit operator override) should take a double and return an operation (a constant)
    {
        public Operation Clone()
        {
            var copy = ShallowClone();
            if(LeftOperand != null)
            {
                copy.LeftOperand = LeftOperand.Clone();
            }
            if (RightOperand != null)
            {
                copy.RightOperand = RightOperand.Clone();
            }
            return copy;
        }

        protected virtual Operation ShallowClone()
        {
            return (Operation)Activator.CreateInstance(this.GetType());
        }

        Operation leftOperand;
        public Operation LeftOperand
        {
            get{ return leftOperand; }
            set { leftOperand = value; }
        }

        Operation rightOperand;
        public Operation RightOperand
        {
            get{ return rightOperand; }
            set{ rightOperand = value; }
        }
        public abstract double Result { get; }

        public static implicit operator Operation(double number)
        {
            Constant constant = new Constant(number);
            return constant;
        }

    }

    public class AddOperation : Operation
    {

        public override string ToString()
        {
            return $"({LeftOperand} + {RightOperand})";
        }
        public override double Result
        {
            get
            {
                if (LeftOperand == null || RightOperand == null)
                    return double.NaN;
                return LeftOperand.Result + RightOperand.Result;
            }
        }
    }

    public class SubtractOperation : Operation
    {

        public override string ToString()
        {
            return $"({LeftOperand} - {RightOperand})";
        }

        public override double Result
        {
            get
            {
                if (LeftOperand == null || RightOperand == null)
                    return double.NaN;
                return LeftOperand.Result - RightOperand.Result;
            }
        }
    }

    public class MultiplyOperation : Operation
    {

        public override string ToString()
        {
            return $"({LeftOperand} * {RightOperand})";
        }
        public override double Result
        {
            get
            {
                if (LeftOperand == null || RightOperand == null)
                    return double.NaN;
                return LeftOperand.Result * RightOperand.Result;
            }
        }
    }

    public class DivideOperation : Operation
    {

        public override string ToString()
        {
            return $"({LeftOperand} / {RightOperand})";
        }
        public override double Result
        {
            get
            {
                if (LeftOperand == null || RightOperand == null)
                    return double.NaN;
                return LeftOperand.Result / RightOperand.Result;
            }
        }
    }

    public class Constant : Operation
    {
        double number;

        protected override Operation ShallowClone()
        {
            return new Constant(number);
        }

        public Constant(double number)
        {
            this.number = number;    
        }
        public override double Result
        {
            get
            {
                return number;
            }
        }
    }
}
