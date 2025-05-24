using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LabyrinthGame
{
    public class TimeAttackUIManager
    {
        private readonly TextBlock timerText;
        private readonly Shape star2;
        private readonly Shape star3;
        private readonly TextBlock coinBalanceText;

        public TimeAttackUIManager(TextBlock timerText, Shape star2, Shape star3, TextBlock coinBalanceText)
        {
            this.timerText = timerText;
            this.star2 = star2;
            this.star3 = star3;
            this.coinBalanceText = coinBalanceText;
        }

        public void UpdateTimerDisplay(TimeSpan time)
        {
            timerText.Text = time.ToString(@"mm\:ss");
        }

        public void UpdateStars(int starsCollected)
        {
            if (starsCollected == 2)
                star3.Fill = new SolidColorBrush(Colors.Gray);
            else if (starsCollected == 1)
                star2.Fill = new SolidColorBrush(Colors.Gray);
        }

        public void UpdateCoinBalance(int coins)
        {
            coinBalanceText.Text = coins.ToString();
        }

        public void ShowNotification(string message, bool isError = false)
        {
            NotificationService.Show(message, isError);
        }
    }
}