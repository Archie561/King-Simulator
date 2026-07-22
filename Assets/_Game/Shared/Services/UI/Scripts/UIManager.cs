using UnityEngine;

namespace Game.Shared.Services.UI
{
    public class UIManager : IUIManager
    {
        public void ShowCanvas(string canvasName)
        {
            // Placeholder: Load and instantiate canvas prefab, or enable existing one
            Debug.Log($"Showing canvas: {canvasName}");
        }

        public void HideCanvas(string canvasName)
        {
            // Placeholder: Disable or destroy canvas
            Debug.Log($"Hiding canvas: {canvasName}");
        }
    }
}
