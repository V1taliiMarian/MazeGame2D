using System.Windows;
using System.Windows.Controls;

namespace LabyrinthGame
{
    public partial class GameModesInfoPage : Page
    {
        public GameModesInfoPage()
        {
            InitializeComponent();
            AudioManager.AddButtonsSound(BackButtonFromInfo);
            EscKeyHandler.AttachToPage(this);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}