using Features.Components;
using Features.Tags;
using Features.Views;
using Leopotam.EcsLite;

namespace Features.Systems
{
    public class UIUpdateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _businessUIFilter;
        private EcsFilter _playerFilter;
        private EcsPool<BusinessUIComponent> _uiPool;
        private EcsPool<BusinessComponent> _businessPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<UpgradeComponent> _upgradePool;
        private EcsPool<CostComponent> _costPool;
        private EcsPool<BusinessNamesComponent> _namesPool;
        private EcsPool<PlayerBalanceComponent> _balancePool;
        private readonly GameUI _gameUI;

        public UIUpdateSystem(GameUI gameUI)
        {
            _gameUI = gameUI;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            _businessUIFilter = world.Filter<BusinessUIComponent>()
                .Inc<BusinessComponent>()
                .Inc<IncomeComponent>()
                .Inc<UpgradeComponent>()
                .Inc<CostComponent>()
                .Inc<BusinessNamesComponent>()
                .End();

            _playerFilter = world.Filter<PlayerBalanceComponent>().Inc<PlayerTag>().End();

            _uiPool = world.GetPool<BusinessUIComponent>();
            _businessPool = world.GetPool<BusinessComponent>();
            _incomePool = world.GetPool<IncomeComponent>();
            _upgradePool = world.GetPool<UpgradeComponent>();
            _costPool = world.GetPool<CostComponent>();
            _namesPool = world.GetPool<BusinessNamesComponent>();
            _balancePool = world.GetPool<PlayerBalanceComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _businessUIFilter)
            {
                ref var ui = ref _uiPool.Get(entity);
                if (ui.View == null)
                    continue;

                ref var business = ref _businessPool.Get(entity);
                ref var income = ref _incomePool.Get(entity);
                ref var upgrade = ref _upgradePool.Get(entity);
                ref var cost = ref _costPool.Get(entity);
                ref var names = ref _namesPool.Get(entity);

                ui.View.UpdateUI(
                    names.BusinessName,
                    business.Level,
                    income.CurrentIncome,
                    income.Progress,
                    cost.CurrentLevelCost,
                    upgrade.Upgrade1Cost,
                    upgrade.Upgrade2Cost,
                    names.Upgrade1Name,
                    names.Upgrade2Name,
                    upgrade.Upgrade1Multiplier,
                    upgrade.Upgrade2Multiplier,
                    upgrade.Upgrade1Bought,
                    upgrade.Upgrade2Bought,
                    business.IsOwned
                );
            }

            foreach (int playerEntity in _playerFilter)
            {
                ref var balance = ref _balancePool.Get(playerEntity);
                _gameUI?.UpdateBalance(balance.Balance);
            }
        }
    }
}