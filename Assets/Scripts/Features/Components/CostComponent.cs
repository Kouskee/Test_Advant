using Leopotam.EcsLite;

namespace Features.Components
{
    public struct CostComponent : IEcsAutoReset<CostComponent>
    {
        public float BaseCost;
        public float CurrentLevelCost;

        public void AutoReset(ref CostComponent c)
        {
            c.BaseCost = 0f;
            c.CurrentLevelCost = 0f;
        }
    }
}