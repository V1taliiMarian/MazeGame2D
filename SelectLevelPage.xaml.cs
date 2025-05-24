using System.Windows;
using System.Windows.Controls;

namespace LabyrinthGame
{
    public partial class SelectLevelPage : Page
    {
        public SelectLevelPage(bool showMainMenuButton = false)
        {
            InitializeComponent();
            ShowMainMenuButton = showMainMenuButton;
            UpdateButtons();
            SetupEvents();
        }

        public bool ShowMainMenuButton { get; set; } = false;

        private void SetupEvents()
        {
            AudioManager.AddButtonsSound(BackButton, RandomGame, TimeAttackGame, InfoButton, MainMenuButton, StatsButton);
        }

        private void UpdateButtons()
        {
            BackButton.Visibility = ShowMainMenuButton ? Visibility.Collapsed : Visibility.Visible;
            MainMenuButton.Visibility = ShowMainMenuButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private async void RandomLevelButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadingService.ShowLoadingAsync(typeof(RandomLevelPage), Window.GetWindow(this));
            NavigationService.Navigate(new RandomLevelPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private async void TimeAttack_Click(object sender, RoutedEventArgs e)
        {
            await LoadingService.ShowLoadingAsync(typeof(TimeAttackPage), Window.GetWindow(this));
            NavigationService.Navigate(new TimeAttackPage());
        }

        private async void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadingService.ShowLoadingAsync(typeof(GameModesInfoPage), Window.GetWindow(this));
            NavigationService?.Navigate(new GameModesInfoPage());
        }

        private async void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadingService.ShowLoadingAsync(typeof(StatisticsPage), Window.GetWindow(this));
            NavigationService?.Navigate(new StatisticsPage());
        }
    }
}