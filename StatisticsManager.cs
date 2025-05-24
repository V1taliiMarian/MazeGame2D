using System;
using System.IO;
using System.Windows;

namespace LabyrinthGame
{
    public class StatisticsManager
    {
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LabyrinthGame");

        private static readonly string StatsFilePath = Path.Combine(AppDataFolder, "stats.txt");

        public int RandomLevelsCompleted { get; private set; }
        public int TimeAttackWins { get; private set; }
        public int TimeAttackLosses { get; private set; }
        public int CoinsCollected { get; private set; }

        public StatisticsManager()
        {
            LoadStats();
        }

        private void LoadStats()
        {
            try
            {
                if (File.Exists(StatsFilePath))
                {
                    var lines = File.ReadAllLines(StatsFilePath);
                    if (lines.Length >= 4)
                    {
                        RandomLevelsCompleted = int.Parse(lines[0]);
                        TimeAttackWins = int.Parse(lines[1]);
                        TimeAttackLosses = int.Parse(lines[2]);
                        CoinsCollected = int.Parse(lines[3]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading stats: {ex.Message}");
            }
        }

        public void SaveStats()
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                    Directory.CreateDirectory(AppDataFolder);

                File.WriteAllText(StatsFilePath,
                    $"{RandomLevelsCompleted}\n{TimeAttackWins}\n{TimeAttackLosses}\n{CoinsCollected}"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving stats: {ex.Message}");
            }
        }

        public void IncrementRandomLevelsCompleted()
        {
            RandomLevelsCompleted++;
            SaveStats();
        }

        public void IncrementTimeAttackWins()
        {
            TimeAttackWins++;
            SaveStats();
        }

        public void IncrementTimeAttackLosses()
        {
            TimeAttackLosses++;
            SaveStats();
        }

        public void IncrementCoinsCollected()
        {
            CoinsCollected++;
            SaveStats();
        }
    }
}   