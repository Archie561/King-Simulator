using System;
using VContainer.Unity;

namespace Game.Shared.Services.Time
{
    /// <summary>
    /// Pure C# class implementing ITimeService and VContainer's ITickable.
    /// Replaces System.Timers to ensure main-thread execution for game loop ticks.
    /// </summary>
    public class TimeService : ITimeService, ITickable
    {
        // Define an event that other modules can subscribe to for per-second ticks.
        public event Action OnOneSecondTick;

        private float _timeSinceLastTick = 0f;

        public DateTime CurrentTime => DateTime.UtcNow;

        public TimeSpan CalculateOfflineTime(DateTime lastSavedTime)
        {
            if (lastSavedTime == DateTime.MinValue)
                return TimeSpan.Zero;
            
            return CurrentTime - lastSavedTime;
        }

        public void Tick()
        {
            // Unity's Time.deltaTime is accessible here, but we can also use UnityEngine.Time.deltaTime.
            _timeSinceLastTick += UnityEngine.Time.deltaTime;

            if (_timeSinceLastTick >= 1.0f)
            {
                _timeSinceLastTick -= 1.0f;
                OnOneSecondTick?.Invoke();
            }
        }
    }
}
