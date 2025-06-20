using Leopotam.EcsLite;
namespace Features.Events
{
    public struct PurchaseLevelEvent : IEcsAutoReset<PurchaseLevelEvent>
    {
        public int BusinessId;

        public void AutoReset(ref PurchaseLevelEvent c)
        {
            c.BusinessId = 0;
        }
    }
}