using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Windows.Input;

namespace SetExpCondition.Controls
{
    public partial class ConditionButton : ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ConditionButton), string.Empty);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ConditionButton));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ConditionButton));

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(ConditionButton), false, propertyChanged: OnIsSelectedChanged);

        public static readonly BindableProperty Factor1Property =
            BindableProperty.Create(nameof(Factor1), typeof(string), typeof(ConditionButton), string.Empty);

        public static readonly BindableProperty Factor2Property =
            BindableProperty.Create(nameof(Factor2), typeof(string), typeof(ConditionButton), string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Factor1
        {
            get => (string)GetValue(Factor1Property);
            set => SetValue(Factor1Property, value);
        }

        public string Factor2
        {
            get => (string)GetValue(Factor2Property);
            set => SetValue(Factor2Property, value);
        }

        public ConditionButton()
        {
            InitializeComponent();
        }

        static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ConditionButton cb && cb.InnerFrame != null)
            {
                var selected = (bool)newValue;
                if (selected)
                {
                    cb.InnerFrame.BackgroundColor = Colors.DodgerBlue;
                    cb.CenterLabel.TextColor = Colors.White;
                    cb.TopLabel.TextColor = Colors.White;
                    cb.BottomLabel.TextColor = Colors.White;
                }
                else
                {
                    cb.InnerFrame.BackgroundColor = Colors.LightGray;
                    cb.CenterLabel.TextColor = Colors.Black;
                    cb.TopLabel.TextColor = Colors.Black;
                    cb.BottomLabel.TextColor = Colors.Black;
                }
            }
        }

        private void OnFrameTapped(object? sender, EventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) == true)
                Command.Execute(CommandParameter);
        }
    }
}