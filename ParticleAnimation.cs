using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LabyrinthGame
{
    public class ParticleAnimation : FrameworkElement, IDisposable
    {
        private readonly Canvas _parentCanvas;
        private readonly List<Particle> _particles = new List<Particle>();
        private readonly DispatcherTimer _timer;
        private readonly Random _random = new Random();
        private readonly DrawingVisual _visual;
        private DrawingContext _context;
        private bool _isDisposed;

        private static readonly Color[] ParticleColors =
        {
            Color.FromRgb(255, 230, 150),
            Color.FromRgb(255, 255, 180),
            Color.FromRgb(200, 200, 200),
            Color.FromRgb(170, 170, 170)
        };

        public ParticleAnimation(Canvas canvas)
        {
            _parentCanvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            _parentCanvas.SizeChanged += Canvas_SizeChanged;

            InitializeParticles();

            _visual = new DrawingVisual();
            AddVisualChild(_visual);
            AddLogicalChild(_visual);

            _parentCanvas.Children.Add(this);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS для стабільності
            };
            _timer.Tick += UpdateAnimation;
            _timer.Start();
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _visual;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isDisposed) return;

            foreach (var particle in _particles)
            {
                particle.UpdateBounds(_parentCanvas.ActualWidth, _parentCanvas.ActualHeight);
            }
        }

        private void InitializeParticles()
        {
            _particles.Clear();

            if (_parentCanvas.ActualWidth <= 0 || _parentCanvas.ActualHeight <= 0)
                return;

            int particleCount = 40;
            double minDistance = Math.Min(_parentCanvas.ActualWidth, _parentCanvas.ActualHeight) / 10;

            for (int i = 0; i < particleCount; i++)
            {
                _particles.Add(CreateParticle(minDistance));
            }
        }

        private Particle CreateParticle(double minDistance)
        {
            int attempts = 0;
            while (attempts < 100)
            {
                double x = _random.NextDouble() * _parentCanvas.ActualWidth;
                double y = _random.NextDouble() * _parentCanvas.ActualHeight;

                bool validPosition = true;
                foreach (var particle in _particles)
                {
                    if (Distance(x, y, particle.X, particle.Y) < minDistance)
                    {
                        validPosition = false;
                        break;
                    }
                }

                if (validPosition)
                {
                    double speed = _random.NextDouble() * 0.8 + 0.5;
                    double angle = _random.NextDouble() * Math.PI * 2;
                    double dirX = Math.Cos(angle);
                    double dirY = Math.Sin(angle);

                    return new Particle(
                        x, y,
                        _random.Next(6, 10),
                        ParticleColors[_random.Next(ParticleColors.Length)],
                        speed,
                        dirX,
                        dirY,
                        _parentCanvas.ActualWidth,
                        _parentCanvas.ActualHeight
                    );
                }
                attempts++;
            }
            return new Particle(0, 0, 8, ParticleColors[0], 0.5, 1, 0, _parentCanvas.ActualWidth, _parentCanvas.ActualHeight);
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            if (_isDisposed || _parentCanvas.ActualWidth <= 0 || _parentCanvas.ActualHeight <= 0)
                return;

            foreach (var particle in _particles)
            {
                particle.Update();
            }

            using (_context = _visual.RenderOpen())
            {
                foreach (var particle in _particles)
                {
                    particle.Draw(_context);
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _timer.Stop();
            _timer.Tick -= UpdateAnimation;
            _parentCanvas.SizeChanged -= Canvas_SizeChanged;
            _parentCanvas.Children.Remove(this);
            _particles.Clear();
        }

        private class Particle
        {
            public double X { get; private set; }
            public double Y { get; private set; }
            public double Size { get; }
            public Color Color { get; }
            public double Speed { get; }
            private double _directionX;
            private double _directionY;
            private double _maxWidth;
            private double _maxHeight;

            public Particle(double x, double y, double size, Color color,
                          double speed, double dirX, double dirY,
                          double maxWidth, double maxHeight)
            {
                X = x;
                Y = y;
                Size = size;
                Color = color;
                Speed = speed;
                _directionX = dirX;
                _directionY = dirY;
                _maxWidth = maxWidth;
                _maxHeight = maxHeight;
            }

            public void UpdateBounds(double newWidth, double newHeight)
            {
                _maxWidth = newWidth;
                _maxHeight = newHeight;
                X = Clamp(X, Size / 2, _maxWidth - Size / 2);
                Y = Clamp(Y, Size / 2, _maxHeight - Size / 2);
            }

            private static double Clamp(double value, double min, double max)
            {
                return (value < min) ? min : (value > max) ? max : value;
            }

            public void Update()
            {
                X += _directionX * Speed;
                Y += _directionY * Speed;

                double halfSize = Size / 2;

                if (X < halfSize)
                {
                    X = halfSize;
                    _directionX = Math.Abs(_directionX);
                }
                else if (X > _maxWidth - halfSize)
                {
                    X = _maxWidth - halfSize;
                    _directionX = -Math.Abs(_directionX);
                }

                if (Y < halfSize)
                {
                    Y = halfSize;
                    _directionY = Math.Abs(_directionY);
                }
                else if (Y > _maxHeight - halfSize)
                {
                    Y = _maxHeight - halfSize;
                    _directionY = -Math.Abs(_directionY);
                }
            }

            public void Draw(DrawingContext dc)
            {
                var dotBrush = new SolidColorBrush(Color);
                var glowBrush = new RadialGradientBrush(
                    new GradientStopCollection {
                        new GradientStop(Color, 0.3),
                        new GradientStop(Color.FromArgb(50, Color.R, Color.G, Color.B), 1)
                    });

                dc.DrawEllipse(glowBrush, null, new Point(X, Y), Size, Size);
                dc.DrawEllipse(dotBrush, null, new Point(X, Y), Size / 2, Size / 2);
            }
        }
    }

    public static class ParticleAnimator
    {
        private static readonly Dictionary<Canvas, ParticleAnimation> _animations =
            new Dictionary<Canvas, ParticleAnimation>();

        public static void AttachToCanvas(Canvas canvas)
        {
            if (canvas == null || _animations.ContainsKey(canvas))
                return;

            var animation = new ParticleAnimation(canvas);
            _animations.Add(canvas, animation);
        }

        public static void DetachFromCanvas(Canvas canvas)
        {
            if (canvas == null || !_animations.TryGetValue(canvas, out var animation))
                return;

            animation.Dispose();
            _animations.Remove(canvas);
        }
    }
}
