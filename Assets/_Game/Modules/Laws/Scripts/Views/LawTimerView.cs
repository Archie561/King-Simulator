using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Modules.Laws.Views
{
    public class LawTimerView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _cardCountText;
        [SerializeField] private Button _buyRefillButton;

        public event Action OnBuyRefillClicked;

        private void Awake()
        {
            _buyRefillButton.onClick.AddListener(() => OnBuyRefillClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _buyRefillButton.onClick.RemoveAllListeners();
        }

        public void UpdateTimerDisplay(float secondsRemaining)
        {
            TimeSpan time = TimeSpan.FromSeconds(secondsRemaining);
            _timerText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
        }

        public void UpdateCardCount(int currentCount, int maxCount)
        {
            _cardCountText.text = $"{currentCount}/{maxCount}";
            // Disable refill button if full
            _buyRefillButton.interactable = currentCount < maxCount;
        }
    }
}
