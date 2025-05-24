using System.Windows;

namespace LabyrinthGame
{
    public partial class ConflictWindow : Window
    {
        public ConflictWindow(string message)
        {
            InitializeComponent();
            ConflictMessageText.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
