using Leopotam.EcsLite;

namespace Features.Components
{
    public struct BusinessNamesComponent : IEcsAutoReset<BusinessNamesComponent>
    {
        public string BusinessName;
        public string Upgrade1Name;
        public string Upgrade2Name;

        public void AutoReset(ref BusinessNamesComponent c)
        {
            c.BusinessName = string.Empty;
            c.Upgrade1Name = string.Empty;
            c.Upgrade2Name = string.Empty;
        }
    }
}