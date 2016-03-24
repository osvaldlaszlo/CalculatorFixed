using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged, ICalculatorView
    {
        CalculatorController controller;

        public CalculatorViewModel()
        {
            controller = new CalculatorController(this);
            numberPressedCommand = new Command(HandleNumberButtonPressed);
            operatorPressedCommand = new Command(HandleOperatorButtonPressed);
            modifierPressedCommand = new Command(HandleModifierButtonPressed);
        }

        string display;
        public string Display
        {
            get { return display; }
            set
            {
                display = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<NumberPressedEventArgs> NumberPressed;
        public event EventHandler<OperatorPressedEventArgs> OperatorPressed;
        public event EventHandler<ModifierPressedEventArgs> ModifierPressed;

        ICommand numberPressedCommand;
        public ICommand NumberPressedCommand
        {
            get { return numberPressedCommand; }
        }

        ICommand operatorPressedCommand;
        public ICommand OperatorPressedCommand
        {
            get { return operatorPressedCommand; }
        }

        ICommand modifierPressedCommand;
        public ICommand ModifierPressedCommand
        {
            get { return modifierPressedCommand; }
        }

        private void HandleNumberButtonPressed(object param)
        {
            
            var val = int.Parse(param.ToString());
            NumberPressed?.Invoke(this, new NumberPressedEventArgs(val));
        }

        private void HandleOperatorButtonPressed(object param)
        {
            var val = (Operator)Enum.Parse(typeof(Operator), param.ToString());
            OperatorPressed?.Invoke(this, new OperatorPressedEventArgs(val));
        }

        private void HandleModifierButtonPressed(object param)
        {
            var val = (Modifier)Enum.Parse(typeof(Modifier), param.ToString());
            ModifierPressed?.Invoke(this, new ModifierPressedEventArgs(val));
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null) //[CallerMemberName] is an attribute 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // ?. means do only if the preceding property is not null
        }
    }
}
