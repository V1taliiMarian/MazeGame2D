using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LabyrinthGame
{
    public class ExitFactory
    {
        private readonly MazeService mazeService;

        public ExitFactory(MazeService mazeService)
        {
            this.mazeService = mazeService;
        }

        public Rectangle CreateExit(Canvas canvas)
        {
            var exit = new Rectangle
            {
                Width = mazeService.CellSize * 0.8,
                Height = mazeService.CellSize * 0.8,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };

            Canvas.SetLeft(exit,
                mazeService.ExitPosition.X * mazeService.CellSize + mazeService.CellSize * 0.1);
            Canvas.SetTop(exit,
                mazeService.ExitPosition.Y * mazeService.CellSize + mazeService.CellSize * 0.1);

            canvas.Children.Add(exit);
            return exit;
        }
    }
}