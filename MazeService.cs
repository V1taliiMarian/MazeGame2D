using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LabyrinthGame
{
    public class MazeService
    {
        public int CellSize { get; private set; }
        public int MazeWidth { get; private set; }
        public int MazeHeight { get; private set; }
        public bool[,] Walls { get; private set; }
        public Point StartPosition { get; private set; }
        public Point ExitPosition { get; private set; }

        public void GenerateMaze(string difficulty)
        {
            SetDifficultyParameters(difficulty);
            MazeGenerator.Generate(MazeWidth, MazeHeight, out bool[,] walls);
            Walls = walls;

            StartPosition = new Point(1, 0);
            Walls[(int)StartPosition.X, (int)StartPosition.Y] = false;

            ExitPosition = new Point(MazeWidth - 2, MazeHeight - 1);
            Walls[(int)ExitPosition.X, (int)ExitPosition.Y] = false;
        }

        public void DrawMaze(Canvas canvas, Color wallColor, Color borderColor)
        {
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            if (Walls == null) throw new InvalidOperationException("Maze must be generated before drawing");

            canvas.Children.Clear();
            canvas.Width = MazeWidth * CellSize;
            canvas.Height = MazeHeight * CellSize;

            for (int x = 0; x < MazeWidth; x++)
            {
                for (int y = 0; y < MazeHeight; y++)
                {
                    if (Walls[x, y])
                    {
                        var wall = new Rectangle
                        {
                            Width = CellSize,
                            Height = CellSize,
                            Fill = new SolidColorBrush(wallColor),
                            Stroke = new SolidColorBrush(borderColor),
                            StrokeThickness = 1
                        };

                        Canvas.SetLeft(wall, x * CellSize);
                        Canvas.SetTop(wall, y * CellSize);
                        canvas.Children.Add(wall);
                    }
                }
            }
        }

        public void CenterMaze(Canvas mazeCanvas, Grid containerGrid)
        {
            if (mazeCanvas == null || containerGrid == null) return;

            Canvas.SetLeft(mazeCanvas, (containerGrid.ActualWidth - mazeCanvas.Width) / 2);
            Canvas.SetTop(mazeCanvas, (containerGrid.ActualHeight - mazeCanvas.Height) / 2);
        }

        public bool IsWallAt(int x, int y)
        {
            if (x < 0 || x >= MazeWidth || y < 0 || y >= MazeHeight)
                return true;

            return Walls[x, y];
        }

        public bool IsPositionValid(Point position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;

            return x >= 0 && x < MazeWidth &&
                   y >= 0 && y < MazeHeight &&
                   !Walls[x, y];
        }

        private void SetDifficultyParameters(string difficulty)
        {
            switch (difficulty)
            {
                case "Легкий":
                    CellSize = 40;
                    MazeWidth = 15;
                    MazeHeight = 15;
                    break;
                case "Середній":
                    CellSize = 30;
                    MazeWidth = 21;
                    MazeHeight = 21;
                    break;
                case "Складний":
                    CellSize = 20;
                    MazeWidth = 31;
                    MazeHeight = 31;
                    break;
                default:
                    CellSize = 30;
                    MazeWidth = 21;
                    MazeHeight = 21;
                    break;
            }
        }
    }
}