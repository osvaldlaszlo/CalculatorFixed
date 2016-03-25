using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Calculator;

namespace CalculatorTests
{
    public class MockView : ICalculatorView
    {
        public string Display { get; set; }

        public event EventHandler<NumberPressedEventArgs> NumberPressed; //magic used to create the "event object"
        public event EventHandler<OperatorPressedEventArgs> OperatorPressed;
        public event EventHandler<ModifierPressedEventArgs> ModifierPressed;

        public void SendNumberPressed(double number)
        {
            NumberPressed?.Invoke(this, new NumberPressedEventArgs(number));
        }

        public void SendOperatorPressed(Operator op)
        {
           OperatorPressed?.Invoke(this, new OperatorPressedEventArgs(op));
        }
        
        public void SendModifierPressed(Modifier mod)
        {
            ModifierPressed?.Invoke(this, new ModifierPressedEventArgs(mod));
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMultiply()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(4);
            mockView.SendOperatorPressed(Operator.Multiply);
            mockView.SendNumberPressed(5);
            mockView.SendModifierPressed(Modifier.Equal);

           Assert.AreEqual("20", mockView.Display);

        }

        [Test]
        public void TestDivide()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(5);
            mockView.SendOperatorPressed(Operator.Divide);
            mockView.SendNumberPressed(3);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("1.6667", mockView.Display);

        }

        [Test]
        public void TestAdd()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(0.33333333333);
            mockView.SendOperatorPressed(Operator.Add);
            mockView.SendNumberPressed(5);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("5.3333", mockView.Display);

        }

        [Test]
        public void TestSubtract()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(4);
            mockView.SendOperatorPressed(Operator.Subtract);
            mockView.SendNumberPressed(5);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("-1", mockView.Display);

        }

        [Test]
        public void TestMultipleDigitInput()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(4);
            mockView.SendNumberPressed(5);

            Assert.AreEqual("45", mockView.Display);
        }

        [Test] //note: need to test for user pressing equal prior to any other operations
        public void TestDecimalInput()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(6);
            mockView.SendModifierPressed(Modifier.Period);
            mockView.SendNumberPressed(7);
            mockView.SendNumberPressed(8);

            Assert.AreEqual("6.78", mockView.Display);
        }

        [Test]
        public void TestInvert()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(2);
            mockView.SendModifierPressed(Modifier.Invert);
            Assert.AreEqual("-2", mockView.Display);

            mockView.SendOperatorPressed(Operator.Add);
            mockView.SendNumberPressed(3);
            mockView.SendModifierPressed(Modifier.Equal);
            Assert.AreEqual("1", mockView.Display);

            mockView.SendModifierPressed(Modifier.Invert);
            Assert.AreEqual("-1", mockView.Display);            
        }

        [Test]
        public void TestSimpleParens() // 3+(3/2) = 
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(3);
            mockView.SendOperatorPressed(Operator.Add);
            mockView.SendModifierPressed(Modifier.OpenParen);
            mockView.SendNumberPressed(3);
            mockView.SendOperatorPressed(Operator.Divide);
            mockView.SendNumberPressed(2);
            mockView.SendModifierPressed(Modifier.ClosedParen);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("4.5", mockView.Display);
        }

        [Test]
        public void TestClear()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(3);
            mockView.SendOperatorPressed(Operator.Add);
            mockView.SendModifierPressed(Modifier.Clear);

            Assert.AreEqual("", mockView.Display);


            //test functionality of calculator after clearing
            mockView.SendNumberPressed(4);
            mockView.SendOperatorPressed(Operator.Subtract);
            mockView.SendNumberPressed(5);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("-1", mockView.Display);
        }

        [Test]
        public void TestSimpleParenDisplay() //3+(4+2) = 
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(3);
            mockView.SendOperatorPressed(Operator.Add);
            Assert.AreEqual("3", mockView.Display);

            mockView.SendModifierPressed(Modifier.OpenParen);
            Assert.AreEqual("0", mockView.Display);

            mockView.SendNumberPressed(4);
            Assert.AreEqual("4", mockView.Display);

            mockView.SendOperatorPressed(Operator.Add);
            Assert.AreEqual("4", mockView.Display);

            mockView.SendNumberPressed(2);
            Assert.AreEqual("2", mockView.Display);

            mockView.SendModifierPressed(Modifier.ClosedParen);
            Assert.AreEqual("6", mockView.Display);

            mockView.SendModifierPressed(Modifier.Equal);
            Assert.AreEqual("9", mockView.Display);

        }

        //More complicated user-entry testing

        [Test]
        public void TestComplexParenMathWithDecimalInput() //1.457 + (4*(8/3)-6) =
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(1);
            mockView.SendModifierPressed(Modifier.Period);
            mockView.SendNumberPressed(4);
            mockView.SendNumberPressed(5);
            mockView.SendNumberPressed(7);
            mockView.SendOperatorPressed(Operator.Add);

            mockView.SendModifierPressed(Modifier.OpenParen);
            mockView.SendNumberPressed(4);
            mockView.SendOperatorPressed(Operator.Multiply);

            mockView.SendModifierPressed(Modifier.OpenParen);
            mockView.SendNumberPressed(8);
            mockView.SendOperatorPressed(Operator.Divide);
            mockView.SendNumberPressed(3);
            mockView.SendModifierPressed(Modifier.ClosedParen);

            mockView.SendOperatorPressed(Operator.Subtract);
            mockView.SendNumberPressed(6);
            mockView.SendModifierPressed(Modifier.ClosedParen);

            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("6.1237", mockView.Display);
        }

        [Test]
        public void TestEntryAfterEqual()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(4);
            mockView.SendOperatorPressed(Operator.Subtract);
            mockView.SendNumberPressed(5);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("-1", mockView.Display);

            mockView.SendOperatorPressed(Operator.Add);
            mockView.SendNumberPressed(4);
            mockView.SendOperatorPressed(Operator.Subtract);
            mockView.SendNumberPressed(5);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("-2", mockView.Display);
        }

        [Test]
        public void TestDoubleEqual()
        {
            var mockView = new MockView();
            var controller = new CalculatorController(mockView);

            mockView.SendNumberPressed(2);
            mockView.SendOperatorPressed(Operator.Multiply);
            mockView.SendNumberPressed(2);
            mockView.SendModifierPressed(Modifier.Equal);
            mockView.SendModifierPressed(Modifier.Equal);

            Assert.AreEqual("8", mockView.Display);

            mockView.SendModifierPressed(Modifier.Equal);
            Assert.AreEqual("16", mockView.Display);

            mockView.SendModifierPressed(Modifier.Equal);
            Assert.AreEqual("32", mockView.Display);
        }
    }
}
