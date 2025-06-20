using Features.Components;
using Features.Events;
using Features.Tags;
using Leopotam.EcsLite;

namespace Features.Systems
{
    public class IncomeEarnedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _earnedEventFilter;
        private EcsFilter _playerFilter;
        private EcsPool<IncomeEarnedEvent> _earnedEventPool;
        private EcsPool<PlayerBalanceComponent> _balancePool;
        private EcsWorld _eventsWorld;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _eventsWorld = systems.GetWorld("events");

            _earnedEventFilter = _eventsWorld.Filter<IncomeEarnedEvent>().End();
            _playerFilter = world.Filter<PlayerBalanceComponent>().Inc<PlayerTag>().End();
            _earnedEventPool = _eventsWorld.GetPool<IncomeEarnedEvent>();
            _balancePool = world.GetPool<PlayerBalanceComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int eventEntity in _earnedEventFilter)
            {
                ref var earnedEvent = ref _earnedEventPool.Get(eventEntity);

                foreach (int playerEntity in _playerFilter)
                {
                    ref var balance = ref _balancePool.Get(playerEntity);
                    balance.Balance += earnedEvent.Amount;
                }

                _eventsWorld.DelEntity(eventEntity);
            }
        }
    }
}