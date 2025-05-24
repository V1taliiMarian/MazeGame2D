using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace LabyrinthGame
{
    public class AchievementManager
    {
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LabyrinthGame");

        private static readonly string AchievementsFilePath = Path.Combine(AppDataFolder, "achievements.txt");

        private readonly CoinManager _coinManager;
        private readonly StatisticsManager _statsManager;

        private readonly List<int> _randomLevelThresholds = new List<int> { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
        private readonly List<int> _timeAttackThresholds = new List<int> { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
        private readonly List<int> _skinThresholds = new List<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };
        private readonly List<int> _skinWallThresholds = new List<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };
        private readonly List<int> _coinsThresholds = new List<int> { 25, 50, 75, 100, 125, 150, 175, 200, 225, 250,
            275, 300, 325, 350, 375, 400, 425, 450, 475, 500 };

        private const int RewardAmount = 5;
        
        private readonly List<int> _claimedRandomLevels = new List<int>();
        private readonly List<int> _claimedTimeAttacks = new List<int>();
        private readonly List<int> _claimedSkins = new List<int>();
        private readonly List<int> _claimedWallSkins = new List<int>();
        private readonly List<int> _claimedCoins = new List<int>();

        public AchievementManager(CoinManager coinManager, StatisticsManager statsManager)
        {
            _coinManager = coinManager;
            _statsManager = statsManager;

            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            LoadAchievements();
        }

        private void LoadAchievements()
        {
            try
            {
                if (File.Exists(AchievementsFilePath))
                {
                    var lines = File.ReadAllLines(AchievementsFilePath);

                    _claimedRandomLevels.Clear();
                    _claimedTimeAttacks.Clear();
                    _claimedSkins.Clear();
                    _claimedWallSkins.Clear();
                    _claimedCoins.Clear();

                    if (lines.Length > 0 && !string.IsNullOrWhiteSpace(lines[0]))
                        _claimedRandomLevels.AddRange(lines[0].Split(',').Select(int.Parse));

                    if (lines.Length > 1 && !string.IsNullOrWhiteSpace(lines[1]))
                        _claimedTimeAttacks.AddRange(lines[1].Split(',').Select(int.Parse));

                    if (lines.Length > 2 && !string.IsNullOrWhiteSpace(lines[2]))
                        _claimedSkins.AddRange(lines[2].Split(',').Select(int.Parse));

                    if (lines.Length > 3 && !string.IsNullOrWhiteSpace(lines[3]))
                        _claimedWallSkins.AddRange(lines[3].Split(',').Select(int.Parse));

                    if (lines.Length > 4 && !string.IsNullOrWhiteSpace(lines[4]))
                        _claimedCoins.AddRange(lines[4].Split(',').Select(int.Parse));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading achievements: {ex.Message}");
                SaveAchievements();
            }
        }

        public void SaveAchievements()
        {
            try
            {
                File.WriteAllText(AchievementsFilePath,
                    $"{string.Join(",", _claimedRandomLevels)}\n" +
                    $"{string.Join(",", _claimedTimeAttacks)}\n" +
                    $"{string.Join(",", _claimedSkins)}\n" +
                    $"{string.Join(",", _claimedWallSkins)}\n" +
                    $"{string.Join(",", _claimedCoins)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving achievements: {ex.Message}");
            }
        }

        public (int current, int nextThreshold, bool canClaim) GetRandomLevelProgress() =>
            GetProgress(_statsManager.RandomLevelsCompleted, _randomLevelThresholds, _claimedRandomLevels);

        public (int current, int nextThreshold, bool canClaim) GetTimeAttackProgress() =>
            GetProgress(_statsManager.TimeAttackWins, _timeAttackThresholds, _claimedTimeAttacks);

        public (int current, int nextThreshold, bool canClaim) GetSkinProgress() =>
            GetProgress(_coinManager.PurchasedSkins.Count, _skinThresholds, _claimedSkins);

        public (int current, int nextThreshold, bool canClaim) GetWallSkinProgress() =>
            GetProgress(_coinManager.PurchasedWallColors.Count, _skinWallThresholds, _claimedWallSkins);

        public (int current, int nextThreshold, bool canClaim) GetCoinsProgress() =>
            GetProgress(_statsManager.CoinsCollected, _coinsThresholds, _claimedCoins);

        private (int current, int nextThreshold, bool canClaim) GetProgress(
            int currentValue,
            List<int> thresholds,
            List<int> claimedThresholds)
        {
            var nextUnclaimed = thresholds
                .FirstOrDefault(t => !claimedThresholds.Contains(t) && currentValue >= t);

            var nextThreshold = nextUnclaimed != 0 ? nextUnclaimed :
                thresholds.FirstOrDefault(t => !claimedThresholds.Contains(t));

            bool canClaim = nextUnclaimed != 0;

            return (currentValue, nextThreshold, canClaim);
        }

        public bool ClaimRandomLevelReward() =>
            ClaimReward(
                _statsManager.RandomLevelsCompleted,
                _randomLevelThresholds,
                _claimedRandomLevels,
                RewardAmount);

        public bool ClaimTimeAttackReward() =>
            ClaimReward(
                _statsManager.TimeAttackWins,
                _timeAttackThresholds,
                _claimedTimeAttacks,
                RewardAmount);

        public bool ClaimSkinReward() =>
            ClaimReward(
                _coinManager.PurchasedSkins.Count,
                _skinThresholds,
                _claimedSkins,
                RewardAmount);

        public bool ClaimWallSkinsReward() =>
            ClaimReward(
                _coinManager.PurchasedWallColors.Count,
                _skinWallThresholds,
                _claimedWallSkins,
                RewardAmount);

        public bool ClaimCoinsReward() =>
            ClaimReward(
                _statsManager.CoinsCollected,
                _coinsThresholds,
                _claimedCoins,
                RewardAmount);

        private bool ClaimReward(
            int currentValue,
            List<int> thresholds,
            List<int> claimedThresholds,
            int rewardAmount)
        {
            var unclaimed = thresholds
                .Where(t => t <= currentValue && !claimedThresholds.Contains(t))
                .ToList();

            if (!unclaimed.Any()) return false;

            var threshold = unclaimed.Min();
            claimedThresholds.Add(threshold);
            _coinManager.AddCoins(rewardAmount);
            SaveAchievements();
            return true;
        }

        public List<int> GetUnclaimedRandomLevels() =>
            _randomLevelThresholds.Where(t => !_claimedRandomLevels.Contains(t)).ToList();

        public List<int> GetUnclaimedTimeAttacks() =>
            _timeAttackThresholds.Where(t => !_claimedTimeAttacks.Contains(t)).ToList();

        public List<int> GetUnclaimedSkins() =>
            _skinThresholds.Where(t => !_claimedSkins.Contains(t)).ToList();
        public List<int> GetUnclaimedWallSkins() =>
            _skinWallThresholds.Where(t => !_claimedSkins.Contains(t)).ToList();

        public List<int> GetUnclaimedCoins() =>
            _coinsThresholds.Where(t => !_claimedCoins.Contains(t)).ToList();
    }
}