using Features.Components;
using Features.Events;
using Features.Tags;
using Leopotam.EcsLite;

namespace Features.Systems
{
    public class PurchaseUpgradeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _purchaseEventFilter;
        private EcsFilter _businessFilter;
        private EcsFilter _playerFilter;
        private EcsPool<PurchaseUpgradeEvent> _purchaseEventPool;
        private EcsPool<BusinessComponent> _businessPool;
        private EcsPool<UpgradeComponent> _upgradePool;
        private EcsPool<PlayerBalanceComponent> _balancePool;
        private EcsPool<RecalculateIncomeEvent> _recalcEventPool;
        private EcsWorld _eventsWorld;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _eventsWorld = systems.GetWorld("events");

            _purchaseEventFilter = _eventsWorld.Filter<PurchaseUpgradeEvent>().End();
            _businessFilter = world.Filter<BusinessComponent>().Inc<UpgradeComponent>().End();
            _playerFilter = world.Filter<PlayerBalanceComponent>().Inc<PlayerTag>().End();

            _purchaseEventPool = _eventsWorld.GetPool<PurchaseUpgradeEvent>();
            _businessPool = world.GetPool<BusinessComponent>();
            _upgradePool = world.GetPool<UpgradeComponent>();
            _balancePool = world.GetPool<PlayerBalanceComponent>();
            _recalcEventPool = _eventsWorld.GetPool<RecalculateIncomeEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int eventEntity in _purchaseEventFilter)
            {
                ref var purchaseEvent = ref _purchaseEventPool.Get(eventEntity);

                foreach (int businessEntity in _businessFilter)
                {
                    ref var business = ref _businessPool.Get(businessEntity);
                    if (business.BusinessId != purchaseEvent.BusinessId)
                        continue;

                    ref var upgrade = ref _upgradePool.Get(businessEntity);

                    float cost = 0f;
                    bool canBuy = false;

                    if (purchaseEvent.UpgradeIndex == 0 && !upgrade.Upgrade1Bought)
                    {
                        cost = upgrade.Upgrade1Cost;
                        canBuy = true;
                    }
                    else if (purchaseEvent.UpgradeIndex == 1 && !upgrade.Upgrade2Bought)
                    {
                        cost = upgrade.Upgrade2Cost;
                        canBuy = true;
                    }

                    if (!canBuy)
                        break;

                    foreach (int playerEntity in _playerFilter)
                    {
                        ref var balance = ref _balancePool.Get(playerEntity);

                        if (balance.Balance >= cost)
                        {
                            balance.Balance -= cost;

                            if (purchaseEvent.UpgradeIndex == 0)
                            {
                                upgrade.Upgrade1Bought = true;
                            }
                            else
                            {
                                upgrade.Upgrade2Bought = true;
                            }

                            int recalcEntity = _eventsWorld.NewEntity();
                            ref var recalcEvent = ref _recalcEventPool.Add(recalcEntity);
                            recalcEvent.BusinessId = business.BusinessId;
                        }
                    }
                    break;
                }

                _eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}