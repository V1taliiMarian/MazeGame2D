using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.Properties;

namespace LabyrinthGame
{
    public partial class SettingsPage : Page
    {
        private bool isWaitingForKey = false;
        private Button currentKeyButton = null;
        private Dictionary<string, Key> keyBindings = new Dictionary<string, Key>();

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
            LoadKeyBindings();
            AudioManager.UpdateVolume();
            SubscribeToEvents();
            SetupEvents();
        }
        private void SettingsControl_Changed(object sender, EventArgs e)
        {
            ApplySettingsImmediately();
        }

        private void SetupEvents()
        {
            AudioManager.AddButtonsSound(MuteCheckBox, NextTrackButton, DifficultyComboBox,
                ResetButton, ExitSettingsClick, MusicVolumeSlider, SfxVolumeSlider, OpenMusicFolder);
            EscKeyHandler.AttachToPage(this);
           
        }

        private void NextTrackButton_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.NextTrack();
        }

        private void ConfigureControlsButton_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlayButtonClickSound();

            if (KeyBindingsPanel.Visibility == Visibility.Visible)
            {
                KeyBindingsPanel.Visibility = Visibility.Collapsed;
                ResetButton.Margin = new Thickness(0, 20, 0, 20);
                KeyConflictMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                KeyBindingsPanel.Visibility = Visibility.Visible;
                ResetButton.Margin = new Thickness(0, 20, 0, 10);
            }
        }

        private void Button_exit(object sender, RoutedEventArgs e)
        {
            AudioManager.PlayButtonClickSound();
            NavigationService.GoBack();
        }

        private void SubscribeToEvents()
        {
            MusicVolumeSlider.ValueChanged += SettingsControl_Changed;
            SfxVolumeSlider.ValueChanged += SettingsControl_Changed;
            MuteCheckBox.Checked += SettingsControl_Changed;
            MuteCheckBox.Unchecked += SettingsControl_Changed;
            DifficultyComboBox.SelectionChanged += SettingsControl_Changed;
        }

        private void LoadSettings()
        {
            MusicVolumeSlider.Value = Settings.Default.MusicVolume;
            SfxVolumeSlider.Value = Settings.Default.SfxVolume;
            MuteCheckBox.IsChecked = Settings.Default.IsMuted;

            foreach (ComboBoxItem item in DifficultyComboBox.Items)
            {
                if (item.Content.ToString() == Settings.Default.Difficulty)
                {
                    DifficultyComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void UpdateKeyButtons()
        {
            ForwardKeyButton.Content = keyBindings["Forward"].ToString();
            BackwardKeyButton.Content = keyBindings["Backward"].ToString();
            LeftKeyButton.Content = keyBindings["Left"].ToString();
            RightKeyButton.Content = keyBindings["Right"].ToString();
        }

        private void ApplySettingsImmediately()
        {
            Settings.Default.MusicVolume = (int)MusicVolumeSlider.Value;
            Settings.Default.SfxVolume = (int)SfxVolumeSlider.Value;
            Settings.Default.IsMuted = MuteCheckBox.IsChecked ?? false;
            Settings.Default.Difficulty = (DifficultyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Settings.Default.Save();
            AudioManager.UpdateVolume();
        }

        private void KeyBindingButton_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlayButtonClickSound();
            if (isWaitingForKey) return;

            currentKeyButton = sender as Button;
            if (currentKeyButton != null)
            {
                currentKeyButton.Content = "Натисніть клавішу...";
                isWaitingForKey = true;
            }
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");

                if (System.IO.Directory.Exists(folderPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = folderPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                else
                {
                    MessageBox.Show("Папка не знайдена!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити папку: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!isWaitingForKey) return;

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.LWin || e.Key == Key.RWin ||
                e.Key == Key.System || e.Key == Key.Escape 
                || e.Key == Key.Back)
            {
                if (e.Key == Key.Escape)
                {
                    currentKeyButton.Content = keyBindings[currentKeyButton.Tag.ToString()].ToString();
                    isWaitingForKey = false;
                    e.Handled = true;
                }
                return;
            }

            string currentAction = currentKeyButton.Tag.ToString();
            Key newKey = e.Key;

            var conflictingActions = keyBindings
                .Where(kv => kv.Value == newKey && kv.Key != currentAction)
                .Select(kv => kv.Key)
                .ToList();

            if (conflictingActions.Any())
            {
                string conflictMessage = $"Клавіша '{newKey}' вже використовується для:\n";
                foreach (var action in conflictingActions)
                {
                    conflictMessage += $"{GetActionName(action)}\n";
                }
                conflictMessage += "\nБудь ласка, виберіть іншу клавішу.";

                ConflictWindow conflictWindow = new ConflictWindow(conflictMessage)
                {
                    Owner = Window.GetWindow(this)
                };
                conflictWindow.ShowDialog();
                return;
            }

            keyBindings[currentAction] = newKey;
            currentKeyButton.Content = newKey.ToString();
            isWaitingForKey = false;
            SaveKeyBindings();
            e.Handled = true;
        }

        private string GetActionName(string action)
        {
            switch (action)
            {
                case "Forward": return "Руху вверх";
                case "Backward": return "Руху вниз";
                case "Left": return "Руху ліворуч";
                case "Right": return "Руху праворуч";
                default: return action;
            }
        }

        private void SaveKeyBindings()
        {
            KeyBindingsManager.SaveKeyBindings(keyBindings);
        }

        private void LoadKeyBindings()
        {
            keyBindings = KeyBindingsManager.LoadKeyBindings();
            UpdateKeyButtons();
        }
       
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlayButtonClickSound();

            Settings.Default.Reset();
            Settings.Default.Save();
            AudioManager.UpdateVolume();

            MusicVolumeSlider.Value = 15;
            SfxVolumeSlider.Value = 75;
            MuteCheckBox.IsChecked = false;
            DifficultyComboBox.SelectedIndex = 1;

            LoadKeyBindings();
            SaveKeyBindings();
        }
       
    }
}