using System;

namespace Game.Modules.Laws.Models
{
    /// <summary>
    /// Pure Domain Model representing the countdown timer for refilling law cards.
    /// </summary>
    public class LawTimerModel
    {
        public float CurrentTimer { get; private set; }

        public event Action<float> OnTimerChanged;
        public event Action OnTimerComplete;

        public void LoadState(float timerValue)
        {
            CurrentTimer = timerValue;
            OnTimerChanged?.Invoke(CurrentTimer);
        }

        public void Tick(float deltaSeconds)
        {
            CurrentTimer -= deltaSeconds;

            if (CurrentTimer <= 0)
            {
                OnTimerComplete?.Invoke();
            }
            else
            {
                OnTimerChanged?.Invoke(CurrentTimer);
            }
        }

        public void ResetTimer(float maxTime)
        {
            CurrentTimer = maxTime;
            OnTimerChanged?.Invoke(CurrentTimer);
        }
    }
}
