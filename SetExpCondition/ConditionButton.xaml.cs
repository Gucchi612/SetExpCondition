using Microsoft.Maui.Controls;

namespace SetExpCondition
{
    public partial class ConditionButton : ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ConditionButton), default(string));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public event EventHandler? Clicked;

        public ConditionButton()
        {
            InitializeComponent();
            BindingContext = this; // ’Ç‰Á
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}