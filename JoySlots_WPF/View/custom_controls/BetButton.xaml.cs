using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JoySlots_WPF.View.custom_controls
{
    /// <summary>
    /// Interaction logic for BetButton.xaml
    /// </summary>
    public partial class BetButton : UserControl
    {
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(BetButton),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444343"))));

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BetAmountProperty =
            DependencyProperty.Register("BetAmount", typeof(string), typeof(BetButton), new PropertyMetadata(string.Empty));

        public string BetAmount
        {
            get { return (string)GetValue(BetAmountProperty); }
            set { SetValue(BetAmountProperty, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BetButton));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public BetButton()
        {
            InitializeComponent();
        }

        protected virtual void OnClick()
        {
            RoutedEventArgs args = new RoutedEventArgs(ClickEvent);
            RaiseEvent(args);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        public void ClearBackgroundColor()
        {
            BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444343"));
        }
    }
}
