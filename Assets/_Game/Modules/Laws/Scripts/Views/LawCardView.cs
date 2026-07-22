using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Game.Modules.Laws.Views
{
    public class LawCardView : MonoBehaviour
    {
        [Header("Card UI Elements")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _cardArtImage;
        [SerializeField] private RectTransform _cardRect;

        [Header("Action Buttons")]
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _rejectButton;

        public event Action OnAcceptClicked;
        public event Action OnRejectClicked;

        private bool _isAnimating = false;
        private Action _pendingStateUpdate;

        private void Awake()
        {
            _acceptButton.onClick.AddListener(() => OnAcceptClicked?.Invoke());
            _rejectButton.onClick.AddListener(() => OnRejectClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _acceptButton.onClick.RemoveAllListeners();
            _rejectButton.onClick.RemoveAllListeners();
            _cardRect.DOKill();
        }

        public void ShowNewCard(string title, string description, Sprite art)
        {
            if (_isAnimating)
            {
                _pendingStateUpdate = () => ApplyNewCardState(title, description, art);
                return;
            }
            ApplyNewCardState(title, description, art);
        }

        private void ApplyNewCardState(string title, string description, Sprite art)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            _cardArtImage.sprite = art;
            _cardArtImage.color = Color.white;

            _acceptButton.interactable = true;
            _rejectButton.interactable = true;

            // Simple DOTween pop-in effect when a new card is shown
            _cardRect.DOKill();
            _cardRect.anchoredPosition = Vector2.zero;
            _cardRect.localScale = Vector3.zero;
            _cardRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void ShowEmptyState()
        {
            if (_isAnimating)
            {
                _pendingStateUpdate = ApplyEmptyState;
                return;
            }
            ApplyEmptyState();
        }

        private void ApplyEmptyState()
        {
            _titleText.text = "No Cards";
            _descriptionText.text = "Wait for more laws to arrive.";
            _cardArtImage.sprite = null;
            _cardArtImage.color = Color.clear;
            
            _acceptButton.interactable = false;
            _rejectButton.interactable = false;
        }

        public void AnimateCardExit(bool accepted)
        {
            _isAnimating = true;
            
            // Prevent multiple clicks while exiting
            _acceptButton.interactable = false;
            _rejectButton.interactable = false;

            float direction = accepted ? 1000f : -1000f;
            
            _cardRect.DOKill();
            _cardRect.DOAnchorPosX(direction, 0.4f)
                .SetEase(Ease.InBack)
                .OnComplete(() => 
                {
                    _isAnimating = false;
                    _cardRect.anchoredPosition = Vector2.zero; // Reset position for next card
                    
                    // Apply any state that the Domain injected while we were animating
                    _pendingStateUpdate?.Invoke();
                    _pendingStateUpdate = null;
                });
        }
    }
}
