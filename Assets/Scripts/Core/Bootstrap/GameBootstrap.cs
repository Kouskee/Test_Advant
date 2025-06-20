using Features.Components;
using Features.Configs;
using Features.Events;
using Features.Systems;
using Features.Systems.Data;
using Features.Tags;
using Features.Views;
using Leopotam.EcsLite;
using UnityEngine;

namespace Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private BusinessConfigSO _businessConfig;
        [SerializeField] private BusinessNamesSO _businessNames;

        [Header("UI References")]
        [SerializeField] private BusinessView[] _businessViews = new BusinessView[5];
        [SerializeField] private GameUI _gameUI;

        private EcsWorld _world;
        private EcsWorld _eventsWorld;
        private IEcsSystems _systems;

        private void Start()
        {
            InitializeECS();
            InitializeGame();
            LoadGame();
        }

        private void InitializeECS()
        {
            _world = new EcsWorld();
            _eventsWorld = new EcsWorld();

            _systems = new EcsSystems(_world, "events");
            _systems.AddWorld(_eventsWorld, "events");

            _systems.Add(new PurchaseLevelSystem());
            _systems.Add(new PurchaseUpgradeSystem());
            _systems.Add(new IncomeCalculationSystem());
            _systems.Add(new IncomeEarnedSystem());
            _systems.Add(new IncomeProgressSystem());
            _systems.Add(new UIUpdateSystem(_gameUI));
            _systems.Add(new SaveSystem());

            _systems.Init();
        }

        private void InitializeGame()
        {
            CreatePlayer();

            for (int i = 0; i < 5; i++)
            {
                CreateBusiness(i);
            }

            if (_gameUI != null)
            {
                _gameUI.Initialize(this);
            }
        }

        private void CreatePlayer()
        {
            int playerEntity = _world.NewEntity();

            var balancePool = _world.GetPool<PlayerBalanceComponent>();
            var tagPool = _world.GetPool<PlayerTag>();

            ref var balance = ref balancePool.Add(playerEntity);
            balance.Balance = 10f;

            tagPool.Add(playerEntity);
        }

        private void CreateBusiness(int businessId)
        {
            int businessEntity = _world.NewEntity();

            var businessPool = _world.GetPool<BusinessComponent>();
            var incomePool = _world.GetPool<IncomeComponent>();
            var upgradePool = _world.GetPool<UpgradeComponent>();
            var costPool = _world.GetPool<CostComponent>();
            var uiPool = _world.GetPool<BusinessUIComponent>();
            var namesPool = _world.GetPool<BusinessNamesComponent>();
            var tagPool = _world.GetPool<BusinessTag>();

            ref var business = ref businessPool.Add(businessEntity);
            business.BusinessId = businessId;
            business.Level = businessId == 0 ? 1 : 0;
            business.IsOwned = businessId == 0;

            ref var income = ref incomePool.Add(businessEntity);
            var config = _businessConfig.Businesses[businessId];
            income.BaseIncome = config.BaseIncome;
            income.IncomeDelay = config.IncomeDelay;
            income.Progress = 0f;

            income.CurrentIncome = business.Level * income.BaseIncome;

            ref var upgrade = ref upgradePool.Add(businessEntity);
            upgrade.Upgrade1Cost = config.Upgrade1Cost;
            upgrade.Upgrade2Cost = config.Upgrade2Cost;
            upgrade.Upgrade1Multiplier = config.Upgrade1Multiplier;
            upgrade.Upgrade2Multiplier = config.Upgrade2Multiplier;
            upgrade.Upgrade1Bought = false;
            upgrade.Upgrade2Bought = false;

            ref var cost = ref costPool.Add(businessEntity);
            cost.BaseCost = config.BaseCost;
            cost.CurrentLevelCost = (business.Level + 1) * cost.BaseCost;

            if (businessId < _businessViews.Length && _businessViews[businessId] != null)
            {
                ref var ui = ref uiPool.Add(businessEntity);
                ui.View = _businessViews[businessId];
                ui.View.Initialize(businessId, this);
            }

            ref var names = ref namesPool.Add(businessEntity);
            if (businessId < _businessNames.Names.Length)
            {
                var nameData = _businessNames.Names[businessId];
                names.BusinessName = nameData.BusinessName;
                names.Upgrade1Name = nameData.Upgrade1Name;
                names.Upgrade2Name = nameData.Upgrade2Name;
            }

            tagPool.Add(businessEntity);
        }

        private void LoadGame()
        {
            if (PlayerPrefs.HasKey("SaveData"))
            {
                string json = PlayerPrefs.GetString("SaveData");
                var saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData != null)
                {
                    LoadGameData(saveData);
                }
            }
        }

        private void LoadGameData(SaveData saveData)
        {
            var playerFilter = _world.Filter<PlayerBalanceComponent>().Inc<PlayerTag>().End();
            var balancePool = _world.GetPool<PlayerBalanceComponent>();

            foreach (int playerEntity in playerFilter)
            {
                ref var balance = ref balancePool.Get(playerEntity);
                balance.Balance = saveData.PlayerBalance;
            }

            var businessFilter = _world.Filter<BusinessComponent>().Inc<IncomeComponent>().Inc<UpgradeComponent>().Inc<CostComponent>().End();
            var businessPool = _world.GetPool<BusinessComponent>();
            var incomePool = _world.GetPool<IncomeComponent>();
            var upgradePool = _world.GetPool<UpgradeComponent>();
            var costPool = _world.GetPool<CostComponent>();
            var recalcEventPool = _eventsWorld.GetPool<RecalculateIncomeEvent>();

            foreach (int businessEntity in businessFilter)
            {
                ref var business = ref businessPool.Get(businessEntity);

                if (business.BusinessId < saveData.Businesses.Length)
                {
                    var businessSave = saveData.Businesses[business.BusinessId];

                    business.Level = businessSave.Level;
                    business.IsOwned = businessSave.IsOwned;

                    ref var income = ref incomePool.Get(businessEntity);
                    income.Progress = businessSave.Progress;

                    ref var upgrade = ref upgradePool.Get(businessEntity);
                    upgrade.Upgrade1Bought = businessSave.Upgrade1Bought;
                    upgrade.Upgrade2Bought = businessSave.Upgrade2Bought;

                    ref var cost = ref costPool.Get(businessEntity);
                    cost.CurrentLevelCost = (business.Level + 1) * cost.BaseCost;

                    int recalcEntity = _eventsWorld.NewEntity();
                    ref var recalcEvent = ref recalcEventPool.Add(recalcEntity);
                    recalcEvent.BusinessId = business.BusinessId;
                }
            }
        }

        public void PurchaseLevel(int businessId)
        {
            var eventPool = _eventsWorld.GetPool<PurchaseLevelEvent>();
            int eventEntity = _eventsWorld.NewEntity();
            ref var purchaseEvent = ref eventPool.Add(eventEntity);
            purchaseEvent.BusinessId = businessId;
        }

        public void PurchaseUpgrade(int businessId, int upgradeIndex)
        {
            var eventPool = _eventsWorld.GetPool<PurchaseUpgradeEvent>();
            int eventEntity = _eventsWorld.NewEntity();
            ref var purchaseEvent = ref eventPool.Add(eventEntity);
            purchaseEvent.BusinessId = businessId;
            purchaseEvent.UpgradeIndex = upgradeIndex;
        }

        public void SaveGame()
        {
            var eventPool = _eventsWorld.GetPool<SaveGameEvent>();
            int eventEntity = _eventsWorld.NewEntity();
            eventPool.Add(eventEntity);
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnDestroy()
        {
            SaveGame();

            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }

            if (_eventsWorld != null)
            {
                _eventsWorld.Destroy();
                _eventsWorld = null;
            }
        }
    }
}