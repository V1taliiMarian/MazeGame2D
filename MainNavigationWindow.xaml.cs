using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace LabyrinthGame
{
    public partial class MainNavigationWindow : NavigationWindow
    {
        public MainNavigationWindow()
        {
            InitializeComponent();
            EscKeyHandler.Attach(this);
            this.Navigate(new LoadingPage());
            AudioManager.PlaySound("loadingsound.mp3", 0.2f);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(4)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Navigate(new MainPage());
            };
            timer.Start();
        }

    }
}