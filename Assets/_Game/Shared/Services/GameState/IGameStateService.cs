using System;

namespace Game.Shared.Services.GameState
{
    public interface IGameStateService
    {
        bool IsPaused { get; }
        event Action<bool> OnPauseStateChanged;

        void PauseGame();
        void ResumeGame();

        void SaveGame();
        void LoadGame();
    }
}
