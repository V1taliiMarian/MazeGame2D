using System.Windows.Controls;
using System.Windows.Media;

namespace LabyrinthGame
{
    public class PlayerFactory
    {
        private readonly MazeService mazeService;
        private readonly CoinManager coinManager;

        public PlayerFactory(MazeService mazeService, CoinManager coinManager)
        {
            this.mazeService = mazeService;
            this.coinManager = coinManager;
        }

        public Player CreatePlayer(Canvas canvas)
        {
            var defaultColor = Color.FromRgb(0x91, 0xA3, 0xB0);
            var playerColor = defaultColor;

            if (!string.IsNullOrEmpty(coinManager.LastSelectedColor) &&
                ColorManager.PlayerColorMap.TryGetValue(coinManager.LastSelectedColor, out var color))
            {
                playerColor = color;
            }


            var player = new Player(mazeService.CellSize)
            {
                Fill = new SolidColorBrush(playerColor)
            };

            Canvas.SetLeft(player.GetVisual(),
                mazeService.StartPosition.X * mazeService.CellSize + mazeService.CellSize * 0.1);
            Canvas.SetTop(player.GetVisual(),
                mazeService.StartPosition.Y * mazeService.CellSize + mazeService.CellSize * 0.1);

            canvas.Children.Add(player.GetVisual());
            return player;
        }
    }
}