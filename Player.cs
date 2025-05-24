using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace LabyrinthGame
{
    public class Player
    {
        private readonly Rectangle visual;
        private double prevX;
        private double prevY;
        private readonly int cellSize;

        public Brush Fill
        {
            get => visual.Fill;
            set => visual.Fill = value;
        }

        public Player(int cellSize)
        {
            this.cellSize = cellSize;
            visual = new Rectangle
            {
                Width = cellSize * 0.8,
                Height = cellSize * 0.8,
                Stroke = Brushes.Gray,
                StrokeThickness = 2
            };
        }

        public void Move(int deltaX, int deltaY)
        {
            prevX = Canvas.GetLeft(visual);
            prevY = Canvas.GetTop(visual);
            Canvas.SetLeft(visual, prevX + deltaX * cellSize);
            Canvas.SetTop(visual, prevY + deltaY * cellSize);
        }

        public void UndoMove()
        {
            Canvas.SetLeft(visual, prevX);
            Canvas.SetTop(visual, prevY);
        }

        public UIElement GetVisual() => visual;
    }
}