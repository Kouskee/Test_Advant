using Leopotam.EcsLite;
namespace Features.Events
{
    public struct RecalculateIncomeEvent : IEcsAutoReset<RecalculateIncomeEvent>
    {
        public int BusinessId;

        public void AutoReset(ref RecalculateIncomeEvent c)
        {
            c.BusinessId = 0;
        }
    }
}