using Features.Components;
using Features.Events;
using Leopotam.EcsLite;
using UnityEngine;

namespace Features.Systems
{
    public class IncomeProgressSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _incomeFilter;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<BusinessComponent> _businessPool;
        private EcsWorld _eventsWorld;
        private EcsPool<IncomeEarnedEvent> _earnedEventPool;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _eventsWorld = systems.GetWorld("events");

            _incomeFilter = world.Filter<IncomeComponent>().Inc<BusinessComponent>().End();
            _incomePool = world.GetPool<IncomeComponent>();
            _businessPool = world.GetPool<BusinessComponent>();
            _earnedEventPool = _eventsWorld.GetPool<IncomeEarnedEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            float deltaTime = Time.deltaTime;

            foreach (int entity in _incomeFilter)
            {
                ref var income = ref _incomePool.Get(entity);
                ref var business = ref _businessPool.Get(entity);

                if (!business.IsOwned || business.Level <= 0)
                    continue;

                income.Progress += deltaTime / income.IncomeDelay;

                if (income.Progress >= 1f)
                {
                    int eventEntity = _eventsWorld.NewEntity();
                    ref var earnedEvent = ref _earnedEventPool.Add(eventEntity);
                    earnedEvent.BusinessId = business.BusinessId;
                    earnedEvent.Amount = income.CurrentIncome;

                    income.Progress = 0f;
                }
            }
        }
    }
}