using Leopotam.EcsLite;

namespace Features.Components
{
    public struct PlayerBalanceComponent : IEcsAutoReset<PlayerBalanceComponent>
    {
        public float Balance;

        public void AutoReset(ref PlayerBalanceComponent c)
        {
            c.Balance = 0f;
        }
    }
}