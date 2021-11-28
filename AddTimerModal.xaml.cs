using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timer
{
    /// <summary>
    /// Interaction logic for AddTimerModal.xaml
    /// </summary>
    public partial class AddTimerModal : UserControl
    {
        public AddTimerModal(MainWindow parent)
        {
            Parent = parent;
            InitializeComponent();
        }

        private new readonly MainWindow Parent;
        private void EventSetter_OnHandler(object sender, MouseWheelEventArgs e)
        {
            bool IsAdding = e.Delta > 0;
            var input = (TextBox)sender;
            var number = int.Parse(input.Text);

            number = IsAdding ? number + 1 : number - 1;

            number = number > 59 ? 0 : number;
            number = number < 0 ? 59 : number;
            input.Text = number.ToString();

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var hours = int.Parse(Hours.Text);
            var minutes = int.Parse(Minutes.Text);
            var seconds = int.Parse(Seconds.Text);
            var title = Title.Text.Length == 0 ? "Без названия" : Title.Text;
            if (hours == 0 && minutes == 0 && seconds == 0)
            {
                return;
            }
            Parent.AddTimerToList(new MainWindow.TimerStruct(hours, minutes, seconds, title));
            Parent.CloseModal();
        }

        private void Title_OnMouseWheel(object sender, MouseWheelEventArgs e)
        { }

        private void Background_OnMouseDown(object sender, MouseButtonEventArgs e) => Parent.CloseModal();
    }
}
