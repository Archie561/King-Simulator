using UnityEngine;

namespace Game.Shared.Services.UI
{
    public interface IUIManager
    {
        void ShowCanvas(string canvasName);
        void HideCanvas(string canvasName);
    }
}
