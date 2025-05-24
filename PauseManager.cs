using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System;

namespace LabyrinthGame
{
    public class PauseManager
    {
        private readonly Grid pauseOverlay;
        private readonly Page page;

        public bool IsPaused { get; private set; }

        public PauseManager(Page page, Grid pauseOverlay)
        {
            this.page = page;
            this.pauseOverlay = pauseOverlay;
            IsPaused = false;
            SetupEvents();
        }

        private void SetupEvents()
        {
            AudioManager.AddButtonsSound();
        }

        public void Pause()
        {
            IsPaused = true;
            pauseOverlay.Visibility = Visibility.Visible;
            pauseOverlay.IsHitTestVisible = true;
            AudioManager.PlayButtonClickSound();
        }

        public void Resume()
        {
            IsPaused = false;
            pauseOverlay.Visibility = Visibility.Collapsed;
            pauseOverlay.IsHitTestVisible = false;
            AudioManager.PlayButtonClickSound();

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                page.Focus();
                Keyboard.Focus(page);
            }), DispatcherPriority.Render);
        }

        public void GoToMainMenu()
        {
            AudioManager.PlayButtonClickSound();
            page.NavigationService?.Navigate(new MainPage());
        }

        public void GoToSelectLevel()
        {
            AudioManager.PlayButtonClickSound();
            page.NavigationService?.Navigate(new SelectLevelPage());
        }
    }
}