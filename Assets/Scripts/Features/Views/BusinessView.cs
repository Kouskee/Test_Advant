using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Views
{
    public class BusinessView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _businessNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _incomeText;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private TextMeshProUGUI _levelUpCostText;

        [Header("Upgrade 1 UI")]
        [SerializeField] private Button _upgrade1Button;
        [SerializeField] private TextMeshProUGUI _upgrade1NameText;
        [SerializeField] private TextMeshProUGUI _upgrade1MultiplierText;
        [SerializeField] private TextMeshProUGUI _upgrade1CostText;

        [Header("Upgrade 2 UI")]
        [SerializeField] private Button _upgrade2Button;
        [SerializeField] private TextMeshProUGUI _upgrade2NameText;
        [SerializeField] private TextMeshProUGUI _upgrade2MultiplierText;
        [SerializeField] private TextMeshProUGUI _upgrade2CostText;

        private int _businessId;
        private GameBootstrap _gameBootstrap;

        public void Initialize(int businessId, GameBootstrap gameBootstrap)
        {
            _businessId = businessId;
            _gameBootstrap = gameBootstrap;

            _levelUpButton.onClick.AddListener(OnLevelUpClicked);
            _upgrade1Button.onClick.AddListener(OnUpgrade1Clicked);
            _upgrade2Button.onClick.AddListener(OnUpgrade2Clicked);

            _progressBar.value = 0f;
        }

        public void UpdateUI(
            string businessName,
            int level,
            float currentIncome,
            float progress,
            float levelUpCost,
            float upgrade1Cost,
            float upgrade2Cost,
            string upgrade1Name,
            string upgrade2Name,
            float upgrade1Multiplier,
            float upgrade2Multiplier,
            bool upgrade1Bought,
            bool upgrade2Bought,
            bool isOwned)
        {
            if (_businessNameText != null)
                _businessNameText.text = $"\"{businessName}\"";

            if (_levelText != null)
                _levelText.text = $"LVL \n{level}";

            if (_incomeText != null)
            {
                if (isOwned && level > 0)
                    _incomeText.text = $"Доход \n{FormatNumber(currentIncome)}$";
                else
                    _incomeText.text = "Не куплено";
            }

            if (_progressBar != null)
            {
                _progressBar.value = progress;
                _progressBar.gameObject.SetActive(isOwned && level > 0);
            }

            if (_levelUpButton != null && _levelUpCostText != null)
            {
                _levelUpCostText.text = $"LVL UP \nЦена: {FormatNumber(levelUpCost)}$";
                _levelUpButton.interactable = true;
            }

            UpdateUpgradeUI(
                _upgrade1Button, _upgrade1NameText, _upgrade1MultiplierText, _upgrade1CostText,
                upgrade1Name, upgrade1Multiplier, upgrade1Cost, upgrade1Bought, isOwned
            );

            UpdateUpgradeUI(
                _upgrade2Button, _upgrade2NameText, _upgrade2MultiplierText, _upgrade2CostText,
                upgrade2Name, upgrade2Multiplier, upgrade2Cost, upgrade2Bought, isOwned
            );
        }

        private void UpdateUpgradeUI(
            Button upgradeButton,
            TextMeshProUGUI nameText,
            TextMeshProUGUI multiplierText,
            TextMeshProUGUI costText,
            string upgradeName,
            float multiplier,
            float cost,
            bool bought,
            bool isOwned)
        {
            if (upgradeButton == null) return;

            if (nameText != null) nameText.text = $"\"{upgradeName}\"";
            if (multiplierText != null) multiplierText.text = $"Доход: + {multiplier * 100}%";

            if (bought)
            {
                if (costText != null) costText.text = "Куплено";
                upgradeButton.interactable = false;
            }
            else
            {
                if (costText != null) costText.text = $"Цена: {FormatNumber(cost)}$";
                upgradeButton.interactable = isOwned;
            }
        }

        private void OnLevelUpClicked()
        {
            _gameBootstrap?.PurchaseLevel(_businessId);
        }

        private void OnUpgrade1Clicked()
        {
            _gameBootstrap?.PurchaseUpgrade(_businessId, 0);
        }

        private void OnUpgrade2Clicked()
        {
            _gameBootstrap?.PurchaseUpgrade(_businessId, 1);
        }

        private string FormatNumber(float number)
        {
            if (number >= 1000000f)
                return $"{number / 1000000f:F1}М";
            else if (number >= 1000f)
                return $"{number / 1000f:F1}К";
            else
                return $"{number:F0}";
        }

        private void OnDestroy()
        {
            if (_levelUpButton != null)
                _levelUpButton.onClick.RemoveListener(OnLevelUpClicked);
            if (_upgrade1Button != null)
                _upgrade1Button.onClick.RemoveListener(OnUpgrade1Clicked);
            if (_upgrade2Button != null)
                _upgrade2Button.onClick.RemoveListener(OnUpgrade2Clicked);
        }
    }
}