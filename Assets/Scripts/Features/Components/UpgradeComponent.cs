using Leopotam.EcsLite;

namespace Features.Components
{
    public struct UpgradeComponent : IEcsAutoReset<UpgradeComponent>
    {
        public bool Upgrade1Bought;
        public bool Upgrade2Bought;
        public float Upgrade1Multiplier;
        public float Upgrade2Multiplier;
        public float Upgrade1Cost;
        public float Upgrade2Cost;

        public void AutoReset(ref UpgradeComponent c)
        {
            c.Upgrade1Bought = false;
            c.Upgrade2Bought = false;
            c.Upgrade1Multiplier = 0f;
            c.Upgrade2Multiplier = 0f;
            c.Upgrade1Cost = 0f;
            c.Upgrade2Cost = 0f;
        }
    }
}