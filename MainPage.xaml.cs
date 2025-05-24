using System.Windows;
using System.Windows.Controls;

namespace LabyrinthGame
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeComponents();
            SetupEventHandlers();
        }

        private void InitializeComponents()
        {
            App.CoinManager.DisplayCoins(CoinCountText);
            AudioManager.PlayBackgroundMusic(); 
            AudioManager.UpdateVolume();
        }

        private void SetupEventHandlers()
        {
            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;

            AudioManager.AddButtonsSound(ExitClick, NewGameClick, SettingsClick, ShopClick);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            ParticleAnimator.AttachToCanvas(AnimationCanvas);
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            ParticleAnimator.DetachFromCanvas(AnimationCanvas);
        }

        private async void NewGame_Click(object sender, RoutedEventArgs e)
        {
            await    LoadingService.ShowLoadingAsync(typeof(SelectLevelPage), Window.GetWindow(this));
            NavigationService.Navigate(new SelectLevelPage());
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var exitWindow = new ExitWindow { Owner = Window.GetWindow(this) };
            if (exitWindow.ShowDialog() == true)
            {
                Application.Current.Shutdown();
            }
        }
        
        private async void ShopClick_Click(object sender, RoutedEventArgs e)
        {
            await LoadingService.ShowLoadingAsync(typeof(StorePage), Window.GetWindow(this));
            NavigationService.Navigate(new StorePage(App.CoinManager));
        }
        
    }
}