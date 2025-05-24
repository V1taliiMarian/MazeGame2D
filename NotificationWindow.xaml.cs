using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LabyrinthGame
{
    public partial class NotificationWindow : Window
    {
        private const int SidePadding = 20;
        private readonly DispatcherTimer _timer;

        public NotificationWindow(string message, bool showButtons = false)
        {
            InitializeComponent();
            MessageText.Text = message;
            MessageText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double requiredWidth = MessageText.DesiredSize.Width + (SidePadding * 2);
            Width = Math.Max(requiredWidth, MinWidth);

            MainContent.Margin = new Thickness(SidePadding);

            if (!showButtons)
            {
                ButtonsRow.Height = new GridLength(0);
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.5) };
                _timer.Tick += (s, e) => Close();
                _timer.Start();
            }
            else
            {
                AudioManager.AddButtonsSound(YesButton, NoButton);
            }

            SizeToContent = SizeToContent.Height;
            MaxHeight = SystemParameters.PrimaryScreenHeight * 0.8;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public bool? ShowDialogWithResult()
        {
            ShowDialog();
            return DialogResult;
        }
    }
}