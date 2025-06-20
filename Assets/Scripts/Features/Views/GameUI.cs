using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace Features.Views
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private Button _saveButton;

        private GameBootstrap _gameBootstrap;

        public void Initialize(GameBootstrap gameBootstrap)
        {
            _gameBootstrap = gameBootstrap;

            if (_saveButton != null)
            {
                _saveButton.onClick.AddListener(OnSaveClicked);
            }
        }

        public void UpdateBalance(float balance)
        {
            if (_balanceText != null)
            {
                _balanceText.text = $"{FormatNumber(balance)}$";
            }
        }

        private void OnSaveClicked()
        {
            _gameBootstrap?.SaveGame();
        }

        private string FormatNumber(float number)
        {
            if (number >= 1000000f)
                return $"{number / 1000000f:F2}M";
            else if (number >= 1000f)
                return $"{number / 1000f:F1}K";
            else
                return $"{number:F0}";
        }

        private void OnDestroy()
        {
            if (_saveButton != null)
                _saveButton.onClick.RemoveListener(OnSaveClicked);
        }
    }
}