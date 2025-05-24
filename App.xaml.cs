using System.IO;
using System;
using System.Windows;

namespace LabyrinthGame
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            AudioManager.Cleanup();
            base.OnExit(e);
        }

        public static CoinManager CoinManager { get; } = new CoinManager();
        public static StatisticsManager StatsManager { get; private set; }
        public static AchievementManager AchievementManager { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (string.IsNullOrEmpty(WpfApp1.Properties.Settings.Default.ForwardKey))
            {
                KeyBindingsManager.ResetToDefaults();
            }
            
            string soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            if (!Directory.Exists(soundsFolder))
            {
                Directory.CreateDirectory(soundsFolder);
            }

            StatsManager = new StatisticsManager();
            AchievementManager = new AchievementManager(CoinManager, StatsManager);
        }
    }
}