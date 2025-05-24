using System;
using System.Windows;
using System.Windows.Controls;

namespace LabyrinthGame
{
    public class PauseButtonHandlers
    {
        private readonly PauseManager pauseManager;

        public PauseButtonHandlers(PauseManager pauseManager)
        {
            this.pauseManager = pauseManager;
        }

        public void Pause(object sender, RoutedEventArgs e) => pauseManager.Pause();
        public void Resume(object sender, RoutedEventArgs e) => pauseManager.Resume();
        public void MainMenu(object sender, RoutedEventArgs e) => pauseManager.GoToMainMenu();
        public void SelectLevel(object sender, RoutedEventArgs e) => pauseManager.GoToSelectLevel();

    }
}
