using Leopotam.EcsLite;

namespace Features.Events
{
    public struct IncomeEarnedEvent : IEcsAutoReset<IncomeEarnedEvent>
    {
        public int BusinessId;
        public float Amount;

        public void AutoReset(ref IncomeEarnedEvent c)
        {
            c.BusinessId = 0;
            c.Amount = 0f;
        }
    }
}