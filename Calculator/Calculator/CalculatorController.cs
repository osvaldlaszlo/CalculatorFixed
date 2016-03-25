using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Calculator //need to re-evaluate current/storedOperation, and how to handle operators after equals
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
        bool HasSomethingBeenPressed = false;

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
            HasSomethingBeenPressed = true;
            entry = Entry.Integer;
            decimalCount = 1;

            switch (e.Operator)
            {
                case Operator.Add: //convert to switches....
                    if (parenFlag) //if inside parentheses, do paren math
                    {
                        UpdateCurrentOperands(new AddOperation(), current, null);
                    } else if (equalFlag) //do math correctly in weird situations ( ...x = + 3 should give x+3)
                    {
                        UpdateCurrentOperands(new AddOperation(), current.Result, null);
                        equalFlag = false;
                    } else if (current.LeftOperand != null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(current, current.LeftOperand, storedOperation);
                        UpdateCurrentOperands(new AddOperation(), current.Result, null);
                    } else if (current.LeftOperand == null && current.RightOperand == null) //first entry (or entry after clear)
                    {
                        UpdateCurrentOperands(new AddOperation(), storedOperation, null);
                    }
                    break;

                case Operator.Subtract:
                    if (parenFlag)
                    {
                        UpdateCurrentOperands(new SubtractOperation(), current, null);
                    } else if (equalFlag)
                    {
                        UpdateCurrentOperands(new SubtractOperation(), current.Result, null);
                        equalFlag = false;
                    } else if (current.LeftOperand != null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(current, current.LeftOperand, storedOperation);
                        UpdateCurrentOperands(new SubtractOperation(), current.Result, null);
                    } else if (current.LeftOperand == null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(new SubtractOperation(), storedOperation, null);
                    }
                    break;

                case Operator.Multiply:
                    if (parenFlag)
                    {
                        UpdateCurrentOperands(new MultiplyOperation(), current, null);
                    } else if (equalFlag)
                    {
                        UpdateCurrentOperands(new MultiplyOperation(), current.Result, null);
                        equalFlag = false;
                    } else if (current.LeftOperand != null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(current, current.LeftOperand, storedOperation);
                        UpdateCurrentOperands(new MultiplyOperation(), current.Result, null);
                    } else if (current.LeftOperand == null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(new MultiplyOperation(), storedOperation, null);
                    }
                    break;

                case Operator.Divide:
                    if (parenFlag)
                    {
                        UpdateCurrentOperands(new DivideOperation(), current, null); 
                    } else if (equalFlag)
                    {
                        UpdateCurrentOperands(new DivideOperation(), current.Result, null);
                        equalFlag = false;
                    } else if (current.LeftOperand != null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(current, current.LeftOperand, storedOperation);
                        UpdateCurrentOperands(new DivideOperation(), current.Result, null);
                    } else if (current.LeftOperand == null && current.RightOperand == null)
                    {
                        UpdateCurrentOperands(new DivideOperation(), storedOperation, null);
                    }
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
                    current = 0;
                    storedOperation = 0;
                    parenStack = new Stack<Operation>();
                    parenFlag = false;
                    break;

                case Modifier.Equal: 
                    if(parenFlag)
                    {
                        this.view.Display = current.Result.ToString("0.####");
                        mode = Mode.Replace;
                        parenFlag = false;
                        break;
                    }

                    if(equalFlag)
                    {
                        UpdateCurrentOperands(current, current.Result, storedOperation);
                    }

                    current.RightOperand = storedOperation;
                    this.view.Display = current.Result.ToString("0.####");
                    equalFlag = true;
                    mode = Mode.Replace;
                    break;

                case Modifier.Period:
                    current = storedOperation;
                    this.view.Display = current.Result.ToString("0.");
                    mode = Mode.Replace;
                    entry = Entry.Decimal;
                    break;

                case Modifier.Invert:
                    if (current.GetType() == typeof(Constant))
                    {
                        current = current.Result * -1;
                        storedOperation = storedOperation.Result*-1;
                    } else if (current.LeftOperand == null && current.RightOperand == null)
                    {
                        storedOperation = storedOperation.Result * -1;
                    } else if (current.LeftOperand != null && current.RightOperand == null)
                    {
                        current.LeftOperand = current.LeftOperand.Result * -1;
                        storedOperation = current.LeftOperand;
                    } else if (current.LeftOperand != null && current.RightOperand != null)
                    {
                        current = current.Result * -1;
                    }
                    this.view.Display = "-" + this.view.Display;
                    break;

                case Modifier.OpenParen:
                    parenStack.Push(current.Clone());
                    current = 0;
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
            HasSomethingBeenPressed = true;

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

        private void UpdateCurrentOperands(Operation operation, Operation leftOp, Operation rightOp)
        {
            operation.LeftOperand = leftOp;
            operation.RightOperand = rightOp;
            current = operation;
            mode = Mode.Replace;
            return;
        }
    }
}
