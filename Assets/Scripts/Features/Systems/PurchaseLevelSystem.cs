using Features.Components;
using Features.Events;
using Features.Tags;
using Leopotam.EcsLite;

namespace Features.Systems
{
    public class PurchaseLevelSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _purchaseEventFilter;
        private EcsFilter _businessFilter;
        private EcsFilter _playerFilter;
        private EcsPool<PurchaseLevelEvent> _purchaseEventPool;
        private EcsPool<BusinessComponent> _businessPool;
        private EcsPool<CostComponent> _costPool;
        private EcsPool<PlayerBalanceComponent> _balancePool;
        private EcsPool<RecalculateIncomeEvent> _recalcEventPool;
        private EcsWorld _eventsWorld;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _eventsWorld = systems.GetWorld("events");

            _purchaseEventFilter = _eventsWorld.Filter<PurchaseLevelEvent>().End();
            _businessFilter = world.Filter<BusinessComponent>().Inc<CostComponent>().End();
            _playerFilter = world.Filter<PlayerBalanceComponent>().Inc<PlayerTag>().End();

            _purchaseEventPool = _eventsWorld.GetPool<PurchaseLevelEvent>();
            _businessPool = world.GetPool<BusinessComponent>();
            _costPool = world.GetPool<CostComponent>();
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

                    ref var cost = ref _costPool.Get(businessEntity);

                    foreach (int playerEntity in _playerFilter)
                    {
                        ref var balance = ref _balancePool.Get(playerEntity);

                        if (balance.Balance >= cost.CurrentLevelCost)
                        {
                            balance.Balance -= cost.CurrentLevelCost;

                            business.Level++;
                            business.IsOwned = true;

                            cost.CurrentLevelCost = (business.Level + 1) * cost.BaseCost;

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