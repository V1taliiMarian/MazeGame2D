using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace LabyrinthGame
{
    public partial class ExitWindow : Window
    {
        public ExitWindow()
        {
            InitializeComponent();
            AudioManager.AddButtonsSound(ButtonYes, ButtonNo, SettingsClick);

            this.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    this.DialogResult = false;
                    this.Close();
                    e.Handled = true;
                }
            };
        }

        private void Button_No(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Yes(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();

            if (Application.Current.MainWindow is NavigationWindow navWindow)
            {
                navWindow.Navigate(new SettingsPage());
            }
        }
    }
}