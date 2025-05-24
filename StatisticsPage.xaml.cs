using System;
using System.Windows;
using System.Windows.Controls;

namespace LabyrinthGame
{
    public partial class StatisticsPage : Page
    {
        private readonly StatisticsManager _statsManager;
        private readonly AchievementManager _achievementManager;

        public StatisticsPage()
        {
            InitializeComponent();
            _statsManager = App.StatsManager;
            _achievementManager = App.AchievementManager;
            Loaded += StatisticsPage_Loaded;
            SetupEvents();
        }

        private void SetupEvents()
        {
            AudioManager.AddButtonsSound(BackButton);
            EscKeyHandler.AttachToPage(this);
            App.CoinManager.DisplayCoins(CoinCountText);
        }

        private void StatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayStatistics();
            DisplayAchievements();
        }

        private void DisplayStatistics()
        {
            StatsPanel.Children.Clear();

            AddStatItem("Випадкові рівні пройдено:", _statsManager.RandomLevelsCompleted.ToString());
            AddStatItem("Часова атака: перемоги", _statsManager.TimeAttackWins.ToString());
            AddStatItem("Часова атака: поразки", _statsManager.TimeAttackLosses.ToString());
            AddStatItem("Придбано скінів гравця:", _achievementManager.GetSkinProgress().current.ToString());
            AddStatItem("Придбано скінів стін:", _achievementManager.GetWallSkinProgress().current.ToString());
            AddStatItem("Зібрано монет:", _statsManager.CoinsCollected.ToString());

        }

        private void DisplayAchievements()
        {
            AchievementsPanel.Children.Clear();

            var (rlCurrent, rlNext, rlCanClaim) = _achievementManager.GetRandomLevelProgress();
            AddAchievementItem("Випадкові рівні:",
                rlNext == 0 ? $"{rlCurrent}" : $"{rlCurrent}/{rlNext}",
                rlCanClaim, () =>
                {
                    if (_achievementManager.ClaimRandomLevelReward())
                    {
                        DisplayAchievements();
                        DisplayStatistics();
                    }
                });

            var (taCurrent, taNext, taCanClaim) = _achievementManager.GetTimeAttackProgress();
            AddAchievementItem("Часова атака:",
                taNext == 0 ? $"{taCurrent}" : $"{taCurrent}/{taNext}",
                taCanClaim, () =>
                {
                    if (_achievementManager.ClaimTimeAttackReward())
                    {
                        DisplayAchievements();
                        DisplayStatistics();
                    }
                });

            var (skCurrent, skNext, skCanClaim) = _achievementManager.GetSkinProgress();
            AddAchievementItem("Придбані скіни гравця:",
                skNext == 0 ? $"{skCurrent}" : $"{skCurrent}/{skNext}",
                skCanClaim, () =>
                {
                    if (_achievementManager.ClaimSkinReward())
                    {
                        DisplayAchievements();
                        DisplayStatistics();
                    }
                });

            var (swCurrent, swNext, swCanClaim) = _achievementManager.GetWallSkinProgress();
            AddAchievementItem("Придбані скіни стін:",
                swNext == 0 ? $"{swCurrent}" : $"{swCurrent}/{swNext}",
                swCanClaim, () =>
                {
                    if (_achievementManager.ClaimWallSkinsReward())
                    {
                        DisplayAchievements();
                        DisplayStatistics();
                    }
                });

            var (coinsCurrent, coinsNext, coinsCanClaim) = _achievementManager.GetCoinsProgress();
            AddAchievementItem("Зібрано монет:",
                coinsNext == 0 ? $"{coinsCurrent}" : $"{coinsCurrent}/{coinsNext}",
                coinsCanClaim, () =>
                {
                    if (_achievementManager.ClaimCoinsReward())
                    {
                        DisplayAchievements();
                        DisplayStatistics();
                    }
                });
                }

        private void AddStatItem(string label, string value)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 5)
            };

            panel.Children.Add(new TextBlock
            {
                Text = label,
                Style = (Style)FindResource("StatTextStyle"),
                FontWeight = FontWeights.Bold
            });

            panel.Children.Add(new TextBlock
            {
                Text = value,
                Style = (Style)FindResource("StatTextStyle")
            });

            StatsPanel.Children.Add(panel);
        }

        private void AddAchievementItem(string label, string progress, bool canClaim, Action claimAction)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 5)
            };

            panel.Children.Add(new TextBlock
            {
                Text = label,
                Style = (Style)FindResource("StatTextStyle"),
                FontWeight = FontWeights.Bold,
                Width = 210
            });

            panel.Children.Add(new TextBlock
            {
                Text = progress,
                Style = (Style)FindResource("StatTextStyle"),
                Width = 80,
                TextAlignment = TextAlignment.Center
            });

           int rewardAmount = 5;

            if (label.Contains("Випадкові рівні"))
            {
                rewardAmount = 10;
            }
            else if (label.Contains("Придбані скіни стін"))
            {
                rewardAmount = 25;
            }

            var button = new Button
            {
                Content = rewardAmount.ToString(),
                Style = (Style)FindResource("RewardButtonStyle"),
                IsEnabled = canClaim
            };

            if (canClaim)
            {
        
                button.Click += (s, e) => claimAction();
            }

            panel.Children.Add(button);
            AchievementsPanel.Children.Add(panel);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}