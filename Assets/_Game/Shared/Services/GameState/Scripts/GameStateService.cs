using System;
using UnityEngine;

namespace Game.Shared.Services.GameState
{
    public class GameStateService : IGameStateService
    {
        public bool IsPaused { get; private set; }
        public event Action<bool> OnPauseStateChanged;

        public void PauseGame()
        {
            if (IsPaused) return;
            IsPaused = true;
            OnPauseStateChanged?.Invoke(IsPaused);
            
            // Optionally scale time, though with custom TimeService we might not need this.
            // Time.timeScale = 0f; 
        }

        public void ResumeGame()
        {
            if (!IsPaused) return;
            IsPaused = false;
            OnPauseStateChanged?.Invoke(IsPaused);
            
            // Time.timeScale = 1f;
        }

        public void SaveGame()
        {
            // Placeholder for save logic using a serialization system (JSON/Binary)
            Debug.Log("Game Saved.");
        }

        public void LoadGame()
        {
            // Placeholder for load logic
            Debug.Log("Game Loaded.");
        }
    }
}
