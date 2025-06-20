using Features.Components;
using Features.Events;
using Leopotam.EcsLite;

namespace Features.Systems
{
    public class IncomeCalculationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _recalcEventFilter;
        private EcsFilter _businessFilter;
        private EcsPool<RecalculateIncomeEvent> _recalcEventPool;
        private EcsPool<BusinessComponent> _businessPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<UpgradeComponent> _upgradePool;
        private EcsWorld _eventsWorld;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _eventsWorld = systems.GetWorld("events");

            _recalcEventFilter = _eventsWorld.Filter<RecalculateIncomeEvent>().End();
            _businessFilter = world.Filter<BusinessComponent>().Inc<IncomeComponent>().Inc<UpgradeComponent>().End();

            _recalcEventPool = _eventsWorld.GetPool<RecalculateIncomeEvent>();
            _businessPool = world.GetPool<BusinessComponent>();
            _incomePool = world.GetPool<IncomeComponent>();
            _upgradePool = world.GetPool<UpgradeComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int eventEntity in _recalcEventFilter)
            {
                ref var recalcEvent = ref _recalcEventPool.Get(eventEntity);

                foreach (int businessEntity in _businessFilter)
                {
                    ref var business = ref _businessPool.Get(businessEntity);
                    if (business.BusinessId != recalcEvent.BusinessId)
                        continue;

                    ref var income = ref _incomePool.Get(businessEntity);
                    ref var upgrade = ref _upgradePool.Get(businessEntity);

                    float multiplier = 1f;
                    if (upgrade.Upgrade1Bought)
                        multiplier += upgrade.Upgrade1Multiplier;
                    if (upgrade.Upgrade2Bought)
                        multiplier += upgrade.Upgrade2Multiplier;

                    income.CurrentIncome = business.Level * income.BaseIncome * multiplier;
                    break;
                }

                _eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}