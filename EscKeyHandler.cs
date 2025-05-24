using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LabyrinthGame
{
    public static class EscKeyHandler
    {
        private static ExitWindow _currentExitWindow;

        public static void Attach(Window window)
        {
            window.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    if (_currentExitWindow != null)
                    {
                        _currentExitWindow.Close();
                        _currentExitWindow = null;
                        e.Handled = true;
                    }
                    else
                    {
                        ShowExitWindow(window);
                        e.Handled = true;
                    }
                }
            };
        }


        public static void AttachToPage(Page page)
        {
            page.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Back)
                {
                    AudioManager.PlaySound("errorsound.mp3");
                    e.Handled = true;
                }
            };
        }
        private static void ShowExitWindow(Window owner)
        {
            _currentExitWindow = new ExitWindow
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            _currentExitWindow.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    _currentExitWindow.DialogResult = false;
                    _currentExitWindow.Close();
                    e.Handled = true;
                }
            };

            _currentExitWindow.Closed += (s, e) =>
            {
                _currentExitWindow = null;
            };

            var result = _currentExitWindow.ShowDialog();
            if (result == true)
            {
                Application.Current.Shutdown();
            }
        }
    }
}