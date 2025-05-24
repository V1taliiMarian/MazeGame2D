using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LabyrinthGame
{
    public class CoinPlacer
    {
        private readonly MazeService mazeService;
        private readonly Canvas canvas;
        private readonly Random random = new Random();

        public CoinPlacer(MazeService mazeService, Canvas canvas)
        {
            this.mazeService = mazeService ?? throw new ArgumentNullException(nameof(mazeService));
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        public List<Rectangle> PlaceCoins(Point startPosition, Point exitPosition, int coinCount)
        {
            var coins = new List<Rectangle>();
            int placedCoins = 0;
            int attempts = 0;
            const int maxAttempts = 1000;

            while (placedCoins < coinCount && attempts < maxAttempts)
            {
                attempts++;

                int x = random.Next(1, mazeService.MazeWidth - 1);
                int y = random.Next(1, mazeService.MazeHeight - 1);

                if (!mazeService.IsWallAt(x, y) &&
                    !IsNearPosition(new Point(x, y), startPosition) &&
                    !IsNearPosition(new Point(x, y), exitPosition))
                {
                    var coin = CreateCoin(x, y);
                    coins.Add(coin);
                    placedCoins++;
                }
            }

            return coins;
        }

        private Rectangle CreateCoin(int x, int y)
        {
            var coin = new Rectangle
            {
                Width = mazeService.CellSize * 0.6,
                Height = mazeService.CellSize * 0.6,
                Fill = Brushes.Gold,
                Stroke = Brushes.DarkGoldenrod,
                StrokeThickness = 1
            };

            Canvas.SetLeft(coin, x * mazeService.CellSize + mazeService.CellSize * 0.2);
            Canvas.SetTop(coin, y * mazeService.CellSize + mazeService.CellSize * 0.2);
            canvas.Children.Add(coin);

            return coin;
        }

        private bool IsNearPosition(Point position, Point target, int radius = 3)
        {
            return Math.Abs(position.X - target.X) <= radius &&
                   Math.Abs(position.Y - target.Y) <= radius;
        }
    }
}