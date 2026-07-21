using System;

namespace Game.Shared.Services.Time
{
    public interface ITimeService
    {
        /// <summary>
        /// Gets the current game time.
        /// </summary>
        DateTime CurrentTime { get; }

        /// <summary>
        /// Calculates the offline time elapsed since the last saved time.
        /// </summary>
        /// <param name="lastSavedTime">The last saved time retrieved from save data.</param>
        /// <returns>The TimeSpan representing offline duration.</returns>
        TimeSpan CalculateOfflineTime(DateTime lastSavedTime);
    }
}
