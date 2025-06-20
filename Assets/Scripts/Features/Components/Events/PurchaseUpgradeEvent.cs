using Leopotam.EcsLite;
namespace Features.Events
{
    public struct PurchaseUpgradeEvent : IEcsAutoReset<PurchaseUpgradeEvent>
    {
        public int BusinessId;
        public int UpgradeIndex;

        public void AutoReset(ref PurchaseUpgradeEvent c)
        {
            c.BusinessId = 0;
            c.UpgradeIndex = 0;
        }
    }
}