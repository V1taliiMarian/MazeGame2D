using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LabyrinthGame
{
    public partial class StorePage : Page
    {
        private Button selectedPlayerButton;
        private Button selectedWallButton;
        private readonly CoinManager _coinManager;
        private readonly ColorStoreService playerColorService;
        private readonly ColorStoreService wallColorService;

        public StorePage(CoinManager coinManager)
        {
            InitializeComponent();
            _coinManager = coinManager;
            _coinManager.OnCoinsUpdated += UpdateCoinsDisplay;

            playerColorService = new ColorStoreService(_coinManager);
            wallColorService = new ColorStoreService(_coinManager, true);

            LoadPlayerColors();
            LoadWallColors();
            SetupEvents();
            UpdateCoinsDisplay(_coinManager.Coins);
        }

        private void SetupEvents()
        {
            AudioManager.AddButtonsSound(BackToMainMenuFromThe);
        }

        private void Scroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            double scrollAmount = 60;
            if (e.Delta < 0)
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollAmount);
            else
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - scrollAmount);
            e.Handled = true;
        }

        private void BackToMainMenuFromTheStore_Button(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void UpdateCoinsDisplay(int coins)
        {
            Dispatcher.Invoke(() => CoinCountText.Text = coins.ToString());
        }

        private void LoadPlayerColors()
        {
            PlayerGrid.Children.Clear();
            PlayerGrid.RowDefinitions.Clear();

            var colors = playerColorService.GetAllColors().ToArray();
            int columns = 3;
            int rows = (int)Math.Ceiling(colors.Length / (double)columns);

            for (int i = 0; i < rows; i++)
                PlayerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (index >= colors.Length) break;

                    var colorInfo = colors[index];
                    CreateColorButton(PlayerGrid, row, col, colorInfo,
                                    ref selectedPlayerButton, PlayerColorButton_Click,
                                    colorInfo.Name == playerColorService.GetSelectedColorName());
                    index++;
                }
            }

            SelectedPlayerTextBlock.Text = $"Обраний: {playerColorService.GetSelectedColorName()}";
        }

        private void LoadWallColors()
        {
            WallGrid.Children.Clear();
            WallGrid.RowDefinitions.Clear();

            var colors = wallColorService.GetAllColors().ToArray();
            int columns = 3;
            int rows = (int)Math.Ceiling(colors.Length / (double)columns);

            for (int i = 0; i < rows; i++)
                WallGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (index >= colors.Length) break;

                    var colorInfo = colors[index];
                    CreateColorButton(WallGrid, row, col, colorInfo,
                                    ref selectedWallButton, WallColorButton_Click,
                                    colorInfo.Name == wallColorService.GetSelectedColorName());
                    index++;
                }
            }

            SelectedWallTextBlock.Text = $"Обраний: {wallColorService.GetSelectedColorName()}";
        }

        private void CreateColorButton(Grid parentGrid, int row, int col, ColorInfo colorInfo, ref Button selectedButton,
                                     RoutedEventHandler clickHandler, bool isSelected)
        {
            var container = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            var colorButton = new Button
            {
                Style = (Style)FindResource("SkinButtonStyle"),
                Tag = colorInfo,
                BorderBrush = colorInfo.IsPurchased ? Brushes.LightGreen
                                                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#536878")),
                Background = new SolidColorBrush(colorInfo.Color),
                Width = 120,
                Height = 120,
                BorderThickness = new Thickness(2)
            };

            if (isSelected)
            {
                colorButton.BorderBrush = Brushes.White;
                selectedButton = colorButton;
            }

            colorButton.Click += clickHandler;

            container.Children.Add(colorButton);
            container.Children.Add(new TextBlock
            {
                Text = colorInfo.Name,
                Style = (Style)FindResource("SkinNameTextStyle"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            });

            if (!colorInfo.IsPurchased)
            {
                var pricePanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                pricePanel.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/coin.png")),
                    Width = 16,
                    Height = 16,
                    Margin = new Thickness(0, 0, 3, 0)
                });

                pricePanel.Children.Add(new TextBlock
                {
                    Text = colorInfo.Price.ToString(),
                    Style = (Style)FindResource("PriceTextStyle")
                });

                container.Children.Add(pricePanel);
            }
            else
            {
                container.Children.Add(new TextBlock
                {
                    Text = "Придбано",
                    Style = (Style)FindResource("PurchasedTextStyle"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                });
            }

            Grid.SetRow(container, row);
            Grid.SetColumn(container, col);
            parentGrid.Children.Add(container);
        }

        private void SelectColor(Button button, ColorInfo colorInfo, ColorStoreService service,
                               ref Button selectedButton, TextBlock selectedTextBlock)
        {
            if (selectedButton != null)
            {
                var prevColorInfo = (ColorInfo)selectedButton.Tag;
                selectedButton.BorderBrush = prevColorInfo.IsPurchased
                    ? Brushes.LightGreen
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#536878"));
            }

            selectedButton = button;
            selectedButton.BorderBrush = Brushes.White;
            selectedTextBlock.Text = $"Обраний: {colorInfo.Name}";

            service.SelectColor(colorInfo.Name);
        }

        private void PlayerColorButton_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlayButtonClickSound();
            HandleColorPurchase(sender, playerColorService, ref selectedPlayerButton, SelectedPlayerTextBlock);
        }

        private void WallColorButton_Click(object sender, RoutedEventArgs e)
        {
            AudioManager.PlayButtonClickSound();
            HandleColorPurchase(sender, wallColorService, ref selectedWallButton, SelectedWallTextBlock);
        }

        private void HandleColorPurchase(object sender, ColorStoreService service,
                                       ref Button selectedButton, TextBlock selectedTextBlock)
        {
            var button = (Button)sender;
            var colorInfo = (ColorInfo)button.Tag;

            if (colorInfo.IsPurchased)
            {
                SelectColor(button, colorInfo, service, ref selectedButton, selectedTextBlock);
                return;
            }

            if (!service.CanAfford(colorInfo.Name))
            {
                AudioManager.PlaySound("nopaysound.mp3", 1.2f);
                NotificationService.Show($"Недостатньо монет!\nВартість: {colorInfo.Price}", true);
                return;
            }

            var confirmWindow = new NotificationWindow($"Купити колір \"{colorInfo.Name}\" за {colorInfo.Price} монет?", true);

            confirmWindow.YesButton.Click += (s, args) => {
                AudioManager.PlaySound("paysound.mp3", 1.2f);
                confirmWindow.DialogResult = true;
                confirmWindow.Close();
            };

            confirmWindow.NoButton.Click += (s, args) => {
                confirmWindow.DialogResult = false;
                confirmWindow.Close();
            };

            bool? result = confirmWindow.ShowDialogWithResult();

            if (result == true && service.TryPurchase(colorInfo.Name))
            {
                colorInfo.IsPurchased = true;
                UpdateColorDisplay(button, colorInfo);
                NotificationService.Show($"Колір \"{colorInfo.Name}\" успішно придбано!");
                SelectColor(button, colorInfo, service, ref selectedButton, selectedTextBlock);
            }
        }

        private void UpdateColorDisplay(Button button, ColorInfo colorInfo)
        {
            var container = (StackPanel)button.Parent;

            var pricePanel = container.Children.OfType<StackPanel>().FirstOrDefault(p => p.Children.OfType<Image>().Any());
            if (pricePanel != null) container.Children.Remove(pricePanel);

            container.Children.Add(new TextBlock
            {
                Text = "Придбано",
                Style = (Style)FindResource("PurchasedTextStyle"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            });

            colorInfo.IsPurchased = true;
            button.BorderBrush = Brushes.LightGreen;
        }
    }
}