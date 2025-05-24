using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace LabyrinthGame
{
    public class CoinManager
    {
        private static readonly string AppDataFolder = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"LabyrinthGame");
        //cmd -> %APPDATA%\LabyrinthGame
        private static readonly string FilePath = System.IO.Path.Combine(AppDataFolder, "coins.txt");
        private static readonly string PurchasedSkinsPath = System.IO.Path.Combine(AppDataFolder, "purchased.txt");
        private static readonly string LastSelectedColorPath = System.IO.Path.Combine(AppDataFolder, "lastcolor.txt");
        private static readonly string PurchasedWallColorsPath = System.IO.Path.Combine(AppDataFolder, "purchasedwalls.txt");
        private static readonly string LastSelectedWallColorPath = System.IO.Path.Combine(AppDataFolder, "lastwallcolor.txt");

        private const int MaxCoins = 99;
        private int _coins;

        public event Action<int> OnCoinsUpdated;
        public List<string> PurchasedSkins { get; } = new List<string>();
        public string LastSelectedColor { get; set; } = string.Empty;
        public List<string> PurchasedWallColors { get; } = new List<string>();
        public string LastSelectedWallColor { get; set; } = string.Empty;

        public int Coins
        {
            get => _coins;
            private set
            {
                if (_coins != value)
                {
                    _coins = Math.Min(value, MaxCoins);
                    OnCoinsUpdated?.Invoke(_coins);
                    SaveCoins();
                }
                else
                {
                    _coins = Math.Min(value, MaxCoins);
                    OnCoinsUpdated?.Invoke(_coins);
                }
            }
        }

        public CoinManager()
        {
            LoadCoins();
            LoadPurchasedSkins();
            LoadLastSelectedColor();
            LoadPurchasedWallColors();
            LoadLastSelectedWallColor();
        }

        public void DisplayCoins(TextBlock textBlock)
        {
            textBlock.Text = Coins.ToString();
            OnCoinsUpdated += (coins) =>
            {
                textBlock.Dispatcher.Invoke(() => textBlock.Text = coins.ToString());
            };
        }

        private void LoadCoins()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string content = File.ReadAllText(FilePath).Trim();
                    if (int.TryParse(content, out int coins))
                    {
                        Coins = coins;
                        return;
                    }
                }
                Coins = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження монет: {ex.Message}");
                Coins = 0;
            }
        }

        private void LoadPurchasedSkins()
        {
            try
            {
                if (File.Exists(PurchasedSkinsPath))
                {
                    PurchasedSkins.AddRange(File.ReadAllLines(PurchasedSkinsPath)
                        .Where(line => !string.IsNullOrWhiteSpace(line)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження куплених скінів: {ex.Message}");
            }
        }

        private void LoadLastSelectedColor()
        {
            try
            {
                if (File.Exists(LastSelectedColorPath))
                {
                    LastSelectedColor = File.ReadAllText(LastSelectedColorPath).Trim();
                }
                else
                {
                    LastSelectedColor = "Білий";
                    if (!Directory.Exists(AppDataFolder))
                        Directory.CreateDirectory(AppDataFolder);
                    File.WriteAllText(LastSelectedColorPath, LastSelectedColor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження останнього кольору: {ex.Message}");
                LastSelectedColor = "Білий";
            }
        }


        private void SaveCoins()
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                    Directory.CreateDirectory(AppDataFolder);
                File.WriteAllText(FilePath, Coins.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження монет: {ex.Message}");
            }
        }


        public void SavePurchasedSkins()
        {
            try
            {
                File.WriteAllLines(PurchasedSkinsPath, PurchasedSkins);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження куплених скінів: {ex.Message}");
            }
        }

        public void SaveLastSelectedColor()
        {
            try
            {
                File.WriteAllText(LastSelectedColorPath, LastSelectedColor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження останнього кольору: {ex.Message}");
            }
        }
        private void LoadPurchasedWallColors()
        {
            try
            {
                if (File.Exists(PurchasedWallColorsPath))
                {
                    PurchasedWallColors.AddRange(File.ReadAllLines(PurchasedWallColorsPath)
                        .Where(line => !string.IsNullOrWhiteSpace(line)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження куплених кольорів стін: {ex.Message}");
            }
        }

        private void LoadLastSelectedWallColor()
        {
            try
            {
                if (File.Exists(LastSelectedWallColorPath))
                {
                    LastSelectedWallColor = File.ReadAllText(LastSelectedWallColorPath).Trim();
                }
                else
                {
                    LastSelectedWallColor = "Сірий";
                    if (!Directory.Exists(AppDataFolder))
                        Directory.CreateDirectory(AppDataFolder);
                    File.WriteAllText(LastSelectedWallColorPath, LastSelectedWallColor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження останнього кольору стін: {ex.Message}");
                LastSelectedWallColor = "Сірий";
            }
        }

        public void SavePurchasedWallColors()
        {
            try
            {
                File.WriteAllLines(PurchasedWallColorsPath, PurchasedWallColors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження куплених кольорів стін: {ex.Message}");
            }
        }

        public void SaveLastSelectedWallColor()
        {
            try
            {
                File.WriteAllText(LastSelectedWallColorPath, LastSelectedWallColor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження останнього кольору стін: {ex.Message}");
            }
        }
        public bool CanAfford(int price) => Coins >= price;

        public bool TryPurchase(int price)
        {
            if (!CanAfford(price)) return false;

            Coins -= price;
            return true;
        }

        public void AddCoins(int amount)
        {
            if (amount > 0) Coins += amount;
        }
        public void UpdateCoinBalance(TextBlock coinTextBlock = null)
        {
            if (coinTextBlock != null)
                coinTextBlock.Text = Coins.ToString();
            OnCoinsUpdated?.Invoke(Coins);
        }
    }
}