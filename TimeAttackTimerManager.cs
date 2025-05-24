using System.Windows.Threading;
using System;

namespace LabyrinthGame
{
    public class TimeAttackTimerManager

        {
             public event Action<int> StarsChanged;
             public event Action<TimeSpan> TimeChanged;
             public event Action TimeExpired;

             private readonly DispatcherTimer timer;
             private TimeSpan remainingTime;
             private int starsCollected;
             private readonly int totalTimeSeconds;
             private readonly int firstStarThreshold;
             private readonly int secondStarThreshold;

            public int StarsCollected => starsCollected;
            public TimeSpan RemainingTime => remainingTime;

        public TimeAttackTimerManager(int totalTimeSeconds, int firstStarThreshold, int secondStarThreshold)
        {
            this.totalTimeSeconds = totalTimeSeconds;
            this.firstStarThreshold = firstStarThreshold;
            this.secondStarThreshold = secondStarThreshold;
            remainingTime = TimeSpan.FromSeconds(totalTimeSeconds);
            starsCollected = 3;

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
        }
        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Reset()
        {
            timer.Stop();
            remainingTime = TimeSpan.FromSeconds(totalTimeSeconds);
            starsCollected = 3;
            TimeChanged?.Invoke(remainingTime);
            StarsChanged?.Invoke(starsCollected);
        }

        public void Resume()
        {
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            TimeChanged?.Invoke(remainingTime);

            if (remainingTime.TotalSeconds <= firstStarThreshold && starsCollected == 3)
            {
                starsCollected = 2;
                StarsChanged?.Invoke(starsCollected);
            }
            else if (remainingTime.TotalSeconds <= secondStarThreshold && starsCollected == 2)
            {
                starsCollected = 1;
                StarsChanged?.Invoke(starsCollected);
            }

            if (remainingTime.TotalSeconds <= 0)
            {
                timer.Stop();
                TimeExpired?.Invoke();
            }
        }
    
    }
}