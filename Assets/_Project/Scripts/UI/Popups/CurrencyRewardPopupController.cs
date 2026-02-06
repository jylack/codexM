using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class CurrencyRewardPopupController
    {
        private readonly GameObject _root;
        private readonly Text _amountText;
        private readonly Button _confirmButton;

        public CurrencyRewardPopupController(Transform parent)
        {
            _root = PopupPrefabBinder.CreateOrFallback(parent, "CurrencyRewardPopup");
            UIFactory.Text(_root.transform, "Currency Reward", new Vector2(0, 140));
            _amountText = UIFactory.Text(_root.transform, string.Empty, new Vector2(0, 30));
            _amountText.rectTransform.sizeDelta = new Vector2(900, 80);

            _confirmButton = UIFactory.Button(_root.transform, "Confirm", new Vector2(0, -110), null);
            SetActive(false);
        }

        public void Open(int amount, Action onConfirm)
        {
            _amountText.text = $"+{amount} Gold";
            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                SetActive(false);
            });

            SetActive(true);
        }

        public void SetActive(bool active) => _root.SetActive(active);
    }
}
