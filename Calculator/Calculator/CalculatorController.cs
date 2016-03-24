using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Calculator //need to re-evaluate current/storedOperation, and how to handle operators after equals testestset
{
    public enum Operator
    {
        Multiply = 0,
        Divide,
        Add,
        Subtract,
        Constant
    }

    public enum Modifier
    {
        Equal,
        Period,
        OpenParen,
        ClosedParen,
        Invert,
        Clear
    }

    public enum Mode
    {
        Replace = 0,
        Append,
    }

    public enum Entry
    {
        Integer = 0,
        Decimal,
    }

    public enum Paren
    {
        Unused = 0,
        Used,
    }

    public interface ICalculatorView
    {
        event EventHandler<NumberPressedEventArgs> NumberPressed;
        event EventHandler<OperatorPressedEventArgs> OperatorPressed;
        event EventHandler<ModifierPressedEventArgs> ModifierPressed;
        string Display { get; set; }
    }

    public class NumberPressedEventArgs : EventArgs
    {
        public NumberPressedEventArgs(double num)
        {
            this.Number = num;
        }

        public double Number { get; set; }
    }

    public class OperatorPressedEventArgs : EventArgs
    {
        public OperatorPressedEventArgs(Operator op)
        {
            this.Operator = op;
        }

        public Operator Operator { get; set; }
    }

    public class ModifierPressedEventArgs : EventArgs //EventArgs contains data for the event
    {
        public ModifierPressedEventArgs(Modifier mod)
        {
            this.Modifier = mod;
        }
        public Modifier Modifier { get; set; }
    }

    public class CalculatorController
    {
        ICalculatorView view;

        Mode mode = Mode.Replace;
        Entry entry = Entry.Integer;
        
        int decimalCount = 1;
        bool parenFlag = false;
        bool equalFlag = false;

        Operation current = 0; //initialize current operation
        Operation storedOperation = 0; //initialize storedOperation

        Stack<Operation> parenStack = new Stack<Operation>();

        public CalculatorController(ICalculatorView view)
        {
            this.view = view;
            this.view.NumberPressed += HandleNumberPressed;
            this.view.OperatorPressed += HandleOperatorPressed;
            this.view.ModifierPressed += HandleModifierPressed;
        }

        private void HandleOperatorPressed(object sender, OperatorPressedEventArgs e)
        {
            entry = Entry.Integer;
            decimalCount = 1;

            switch (e.Operator)
            {
                case Operator.Add:
                    if (parenFlag || equalFlag)
                    {
                        NewOperation(new AddOperation(), current);
                    } else
                        NewOperation(new AddOperation(), storedOperation);
                    break;

                case Operator.Subtract:
                    if (parenFlag || equalFlag)
                    {
                        NewOperation(new SubtractOperation(), current);
                    } else
                        NewOperation(new SubtractOperation(), storedOperation);
                    break;

                case Operator.Multiply:
                    if (parenFlag || equalFlag)
                    {
                        NewOperation(new MultiplyOperation(), current);
                    } else
                        NewOperation(new MultiplyOperation(), storedOperation);
                    break;

                case Operator.Divide:
                    if (parenFlag || equalFlag)
                    {
                        NewOperation(new DivideOperation(), current);
                    } else
                        NewOperation(new DivideOperation(), storedOperation);
                    break;
            }
        }

        private void HandleModifierPressed(object sender, ModifierPressedEventArgs e)
        {
            switch (e.Modifier)
            {
                case Modifier.Clear:
                    this.view.Display = "";
                    mode = Mode.Replace;
                    current = null;
                    storedOperation = null;
                    parenStack = null;
                    parenFlag = false;
                    equalFlag = false;
                    break;

                case Modifier.Equal: 
                    if(parenFlag)
                    {
                        this.view.Display = current.Result.ToString("0.####");
                        mode = Mode.Replace;
                        parenFlag = false;
                        break;
                    }

                    current.RightOperand = storedOperation;
                    this.view.Display = current.Result.ToString("0.####");
                    mode = Mode.Replace;
                    equalFlag = true;
                    break;

                case Modifier.Period:
                    current = storedOperation;
                    this.view.Display = current.Result.ToString("0.####");
                    mode = Mode.Replace;
                    entry = Entry.Decimal;
                    break;

                case Modifier.Invert:
                    current = current.Result * -1;
                    this.view.Display = current.Result.ToString("0.####");
                    break;

                case Modifier.OpenParen:
                    parenStack.Push(current.Clone());
                    mode = Mode.Replace;
                    this.view.Display = "0";
                    break;

                case Modifier.ClosedParen:
                    current.RightOperand = storedOperation;
                    var old = parenStack.Pop();
                    old.RightOperand = current;
                    this.view.Display = current.Result.ToString("0.####");
                    current = old;
                    mode = Mode.Replace;
                    parenFlag = true;
                    break;

            }
        }

        private void HandleNumberPressed(object sender, NumberPressedEventArgs e)
        {

            if (entry == Entry.Decimal) 
            {

                storedOperation = storedOperation.Result + e.Number / Math.Pow(10, decimalCount);
                decimalCount++;
                this.view.Display = storedOperation.Result.ToString("0.####");
                return;
            }

            if(mode == Mode.Replace)
            {
                storedOperation = e.Number;
                mode = Mode.Append;
                this.view.Display = storedOperation.Result.ToString("0.####");
            } else
            {
                storedOperation = storedOperation.Result * 10 + e.Number;
                this.view.Display = storedOperation.Result.ToString("0.####");
            }
            
        }

        private void NewOperation(Operation operation, Operation newOperand)
        {
            operation.LeftOperand = newOperand;
            current = operation;
            mode = Mode.Replace;
            return;
        }
    }
}
