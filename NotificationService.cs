using System.Windows.Media;

namespace LabyrinthGame
{
    public static class NotificationService
    {
        public static void Show(string message, bool isError = false)
        {
            var notification = new NotificationWindow(message)
            {
                Background = isError
                    ? new SolidColorBrush(Color.FromRgb(0x91, 0x3A, 0x3A))
                    : new SolidColorBrush(Color.FromRgb(0x91, 0xA3, 0xB0))
            };
            notification.Show();
        }
    }
}