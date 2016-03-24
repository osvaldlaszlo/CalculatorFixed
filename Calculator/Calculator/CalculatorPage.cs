using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Calculator
{
    
    public class CalculatorPage : ContentPage //needs to be re-written to separate the concept of "display" and "solution" and behavior of buttons needs to be reworked to allow for order of operations
    {
        //goal - rewrite solution into another class, calculator, that has add, subtract, multiply, divide, set left/right operand, compute, etc
        //model - Calculator
        //view - CalculatorView - should do things such as CalculatorButtonPressed, showing what button was pressed (condensing events)
        //controller - CalculatorController

        double leftOperand;
        double rightOperand;
        bool repeatEquals;

        bool clearHistory;

        //bool operatorPressed;

        bool decimalEntry;
        int decimalCount = 1;

        //bool parenEntry;

        Operator currentOperator;
        Mode currentMode;

        string history = "";
        string History
        {
            get
            {
                return history;
            }
            set
            {
                history = value.ToString();
                historyLabel.Text = history;
            }
        }

        double display = 0;
        double Display
        {
            get
            {
                return display;
            }
            set
            {
                //if(parenEntry)
                //{
                //    display = value;
                //    label.Text = string.Format("({0:0.#######})", value);
                //}
                display = value;
                label.Text = string.Format("{0:0.#######}",value);
            }
        }

        //need to create local variables instead of instance variables

        Button buttonClear = new Button();
        Button buttonZero = new Button();
        Button buttonEquals = new Button();
        Button buttonDecimal = new Button();
        Button buttonPlusMinus = new Button();

        Button buttonLeftParen = new Button();
        Button buttonRightParen = new Button();

        Button buttonMultiply = new Button();
        Button buttonDivide = new Button();
        Button buttonAdd = new Button();
        Button buttonSubtract = new Button();

        Grid grid = new Grid();
        Label historyLabel = new Label();
        Label label = new Label();
        StackLayout gridWrapper = new StackLayout();

        public CalculatorPage()
        {

            createButtonText();

            Display = 0; //initialize display
            History = ""; //initialize history
            clearHistory = false;
            decimalEntry = false;

            buttonMultiply.Clicked += (s, e) => HandleOperatorButtonClicked(buttonMultiply, Operator.Multiply);
            buttonDivide.Clicked += (s, e) => HandleOperatorButtonClicked(buttonDivide, Operator.Divide);
            buttonAdd.Clicked += (s, e) => HandleOperatorButtonClicked(buttonAdd, Operator.Add);
            buttonSubtract.Clicked += (s, e) => HandleOperatorButtonClicked(buttonSubtract, Operator.Subtract);

            buttonClear.Clicked += (s, e) =>
            {
                Display = 0;
                History = "";
            };

            buttonZero.Clicked += (s, e) => HandleNumberButtonClicked(buttonZero, 0);

            buttonEquals.Clicked += (s, e) =>
            {
                if (clearHistory)
                {
                    History = "";
                    clearHistory = false;
                }

                clearHistory = true;

                if (decimalEntry)
                {
                    decimalCount = 1;
                    decimalEntry = false;
                }

                if (repeatEquals)
                {
                    Display = MathUtils.Compute(Display, rightOperand, currentOperator); //may want to rewrite this to go re-press the buttons to update the history, history is left un-updated for now
                    return;
                }

                rightOperand = Display;
                Display = MathUtils.Compute(leftOperand, Display, currentOperator);
                repeatEquals = true;
                //operatorPressed = false;
            };

            buttonDecimal.Clicked += (s, e) =>
            {
                decimalEntry = true;
                Display = Display + 0;
                History += ".";
            };

            buttonPlusMinus.Clicked += (s, e) =>
            {
                Display = Display * -1;
            };

            gridWrapper.Children.Add(historyLabel);
            gridWrapper.Children.Add(label);
            gridWrapper.Children.Add(grid);

            label.LineBreakMode = LineBreakMode.NoWrap;
            label.FontSize = 50;
            label.HorizontalTextAlignment = TextAlignment.End;

            grid.Padding= new Thickness(5,5,5,5);
            gridWrapper.Padding = new Thickness(30,30,30,30);
            Content = gridWrapper;
        }

        //private void PressEquals()
        //{
        //    buttonEquals.Clicked += (s, e) =>
        //    {
        //        if (clearHistory)
        //        {
        //            History = "";
        //            clearHistory = false;
        //        }

        //        clearHistory = true;

        //        if (decimalEntry)
        //        {
        //            decimalCount = 1;
        //            decimalEntry = false;
        //        }

        //        if (repeatEquals)
        //        {
        //            Display = MathUtils.Compute(Display, rightOperand, currentOperator); //may want to rewrite this to go re-press the buttons to update the history, history is left un-updated for now
        //            return;
        //        }

        //        rightOperand = Display;
        //        Display = MathUtils.Compute(leftOperand, Display, currentOperator);
        //        repeatEquals = true;
        //        operatorPressed = false;
        //    };
        //}

        private void HandleNumberButtonClicked(Button button, int number)
        {
            repeatEquals = false;

            if (clearHistory)
            {
                History = "";
                clearHistory = false;
            }

            if(currentMode == Mode.Replace)
            {
                repeatEquals = false;

                Display = number;
                History += number.ToString();
                currentMode = Mode.Append;
                return;
            }
          
            if (label.Text.Length < 9 && !decimalEntry)
            {
                Display = Display * 10 + number;
                History += number;
            }

            if(label.Text.Length < 9 && decimalEntry)
            {
                Display = Display + number / (Math.Pow(10, decimalCount));
                History += number;
                decimalCount++;
            }
        }

        private void HandleOperatorButtonClicked(Button button, Operator op)
        {
            //if (operatorPressed)
            //    PressEquals();

            //operatorPressed = true;

            if (clearHistory)
            {
                History = "";
                clearHistory = false;
            }

            if (decimalEntry)
            {
                decimalCount = 1;
                decimalEntry = false;
            }

            currentMode = Mode.Replace;
            leftOperand = Display;
            History = leftOperand.ToString();
            if(!History.EndsWith(button.Text))
            {
                History += " " + button.Text + " ";
            }
            currentOperator = op;
        }

        private void createButtonText()
        {
            int x = 9;

            for (int i = 1; i < 4; i++)
            {
                for (int j = 4; j > 0; j--)
                {
                    if (j == 4)
                    {
                        continue;
                    }
                    var tempButton = new Button();
                    tempButton.Text = x.ToString();
                    int val = x;
                    tempButton.Clicked += (s, e) => HandleNumberButtonClicked(tempButton, val);
                    x--;
                    grid.Children.Add(tempButton, j - 1, i);
                }
            }

            //grid.BackgroundColor = Color.Blue;
            grid.VerticalOptions = LayoutOptions.EndAndExpand;

            grid.Children.Add(buttonDivide, 3, 0);
            grid.Children.Add(buttonMultiply, 3, 1);
            grid.Children.Add(buttonSubtract, 3, 2);
            grid.Children.Add(buttonAdd, 3, 3);

            grid.Children.Add(buttonClear, 0, 0);
            grid.Children.Add(buttonZero, 1, 4);
            grid.Children.Add(buttonEquals, 3, 4);
            grid.Children.Add(buttonDecimal, 2, 4);
            grid.Children.Add(buttonPlusMinus, 0, 4);

            grid.Children.Add(buttonLeftParen, 1, 0);
            grid.Children.Add(buttonRightParen, 2, 0);

            buttonMultiply.Text = "*";
            buttonDivide.Text = "/";
            buttonAdd.Text = "+";
            buttonSubtract.Text = "-";

            buttonClear.Text = "C";
            buttonZero.Text = "0";
            buttonEquals.Text = "=";
            buttonDecimal.Text = ".";
            buttonPlusMinus.Text = "\u00B1";

            buttonLeftParen.Text = "(";
            buttonRightParen.Text = ")";

        }
    }
}

