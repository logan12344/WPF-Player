using System.Windows;
using System.Windows.Input;

namespace Player
{
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
        }

        private void TitleBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
