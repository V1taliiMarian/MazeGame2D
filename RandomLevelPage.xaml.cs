using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.Properties;

namespace LabyrinthGame
{
    public partial class RandomLevelPage : Page
    {
        private PauseManager pauseManager;
        private Player player;
        private Rectangle exit;
        private List<Rectangle> coins = new List<Rectangle>();
        private Dictionary<string, Key> keyBindings = new Dictionary<string, Key>();
        private readonly HashSet<Key> pressedKeys = new HashSet<Key>();
        private Canvas mazeCanvas;
        private MazeService mazeService;
        private CoinPlacer coinPlacer;
        private PlayerFactory playerFactory;
        private ExitFactory exitFactory;
        private CollisionManager collisionManager;
        private PauseButtonHandlers pauseHandlers;
        private int collectedCoins = 0;
        private int initialCoins = 0;

        public RandomLevelPage()
        {
            InitializeComponent();
            InitializeServices();
            InitializeGame();
            SetupEvents();
            UpdateCoinBalance();
            this.Loaded += (s, e) => mazeCanvas.Focus();

        }

        private void InitializeServices()
        {
            mazeService = new MazeService();
            playerFactory = new PlayerFactory(mazeService, App.CoinManager);
            exitFactory = new ExitFactory(mazeService);
            pauseManager = new PauseManager(this, PauseOverlay);
            pauseHandlers = new PauseButtonHandlers(pauseManager);
            LoadKeyBindings();
        }

        private void LoadKeyBindings()
        {
            keyBindings = KeyBindingsManager.LoadKeyBindings();
            KeyBindingsManager.KeyBindingsChanged += () =>
            keyBindings = KeyBindingsManager.LoadKeyBindings(); 
        }

        private void InitializeGame()
        {
            collectedCoins = 0;
            initialCoins = App.CoinManager.Coins;
            CreateMazeCanvas();
            mazeService.GenerateMaze(Settings.Default.Difficulty);

            var wallColorInfo = ColorManager.WallColorMap[App.CoinManager.LastSelectedWallColor];
            mazeService.DrawMaze(mazeCanvas, wallColorInfo.Fill, wallColorInfo.Border);

            player = playerFactory.CreatePlayer(mazeCanvas);
            exit = exitFactory.CreateExit(mazeCanvas);

            coinPlacer = new CoinPlacer(mazeService, mazeCanvas);
            PlaceCoins();

            InitializeCollisionManager();
        }

        private void CreateMazeCanvas()
        {
            if (mazeCanvas != null)
            {
                ContentGrid.Children.Remove(mazeCanvas);
            }

            mazeCanvas = new Canvas
            {
                Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x34, 0x39)),
                Focusable = true
            };

            ContentGrid.Children.Insert(0, mazeCanvas);
            mazeCanvas.Focus();
        }

        private void InitializeCollisionManager()
        {
            collisionManager = new CollisionManager(
                mazeService,
                mazeCanvas,
                player,
                exit,
                coins,
                App.CoinManager,
                App.StatsManager);

            collisionManager.OnExitReached += HandleExitReached;
            collisionManager.OnCoinCollected += UpdateCoinBalance; 
        }

        private void PlaceCoins()
        {
            coins.Clear();
            int coinCount = GetCoinCountForDifficulty();
            coins = coinPlacer.PlaceCoins(mazeService.StartPosition, mazeService.ExitPosition, coinCount);
        }

        private int GetCoinCountForDifficulty()
        {
            switch (Settings.Default.Difficulty)
            {
                case "Легкий": return 2;
                case "Середній": return 3;
                case "Складний": return 7;
                default: return 3;
            }
        }

        private void SetupEvents()
        {
            AudioManager.AddButtonsSound(PauseButton, ResumeButton, MainButton, SelectLvlButton);
            EscKeyHandler.AttachToPage(this);

            mazeCanvas.PreviewKeyDown += OnKeyDown;
            mazeCanvas.PreviewKeyUp += OnKeyUp;
            this.SizeChanged += OnPageSizeChanged;
            this.LostFocus += (s, e) => this.Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (pauseManager.IsPaused) return;

            int deltaX = 0, deltaY = 0;

            if (e.Key == keyBindings["Forward"]) deltaY = -1;
            else if (e.Key == keyBindings["Backward"]) deltaY = 1;
            else if (e.Key == keyBindings["Left"]) deltaX = -1;
            else if (e.Key == keyBindings["Right"]) deltaX = 1;

            if (deltaX != 0 || deltaY != 0)
            {
                player.Move(deltaX, deltaY);
                collisionManager.CheckAllCollisions();
                e.Handled = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Key);
        }

        private void HandleExitReached()
        {
            Dispatcher.Invoke(() =>
            {
                App.StatsManager.IncrementRandomLevelsCompleted();
                NotificationService.Show($"Рівень пройдено!\nТи молодець!\nЗібрано монет: {collectedCoins}.", false);

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.5) };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    NavigationService?.Navigate(new SelectLevelPage(true));
                };
                timer.Start();
            });
        }

        private void UpdateCoinBalance()
        {
            App.CoinManager.UpdateCoinBalance(CoinBalanceText);
            collectedCoins = App.CoinManager.Coins - initialCoins; 
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mazeCanvas != null)
            {
                mazeService.CenterMaze(mazeCanvas, ContentGrid);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e) => pauseHandlers.Pause(sender, e);
        private void Button_Resume(object sender, RoutedEventArgs e)
        {
            pauseHandlers.Resume(sender, e);
            mazeCanvas.Focus();
        }
        private void Button_Select(object sender, RoutedEventArgs e) => pauseHandlers.SelectLevel(sender, e);
        private void Button_MainMenu(object sender, RoutedEventArgs e) => pauseHandlers.MainMenu(sender, e);
    }
}