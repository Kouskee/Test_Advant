using Features.Components;
using Features.Events;
using Features.Systems.Data;
using Features.Tags;
using Leopotam.EcsLite;
using UnityEngine;

namespace Features.Systems
{
    public class SaveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _saveEventFilter;
        private EcsFilter _businessFilter;
        private EcsFilter _playerFilter;
        private EcsPool<BusinessComponent> _businessPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<UpgradeComponent> _upgradePool;
        private EcsPool<PlayerBalanceComponent> _balancePool;
        private EcsWorld _eventsWorld;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _eventsWorld = systems.GetWorld("events");

            _saveEventFilter = _eventsWorld.Filter<SaveGameEvent>().End();
            _businessFilter = world.Filter<BusinessComponent>().Inc<IncomeComponent>().Inc<UpgradeComponent>().End();
            _playerFilter = world.Filter<PlayerBalanceComponent>().Inc<PlayerTag>().End();

            _businessPool = world.GetPool<BusinessComponent>();
            _incomePool = world.GetPool<IncomeComponent>();
            _upgradePool = world.GetPool<UpgradeComponent>();
            _balancePool = world.GetPool<PlayerBalanceComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int eventEntity in _saveEventFilter)
            {
                SaveGameData();
                _eventsWorld.DelEntity(eventEntity);
            }
        }

        private void SaveGameData()
        {
            var saveData = new SaveData();

            foreach (int playerEntity in _playerFilter)
            {
                ref var balance = ref _balancePool.Get(playerEntity);
                saveData.PlayerBalance = balance.Balance;
            }

            saveData.Businesses = new BusinessSaveData[5];

            foreach (int businessEntity in _businessFilter)
            {
                ref var business = ref _businessPool.Get(businessEntity);
                ref var income = ref _incomePool.Get(businessEntity);
                ref var upgrade = ref _upgradePool.Get(businessEntity);

                saveData.Businesses[business.BusinessId] = new BusinessSaveData
                {
                    Level = business.Level,
                    IsOwned = business.IsOwned,
                    Progress = income.Progress,
                    Upgrade1Bought = upgrade.Upgrade1Bought,
                    Upgrade2Bought = upgrade.Upgrade2Bought
                };
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString("SaveData", json);
            PlayerPrefs.Save();
        }
    }
}