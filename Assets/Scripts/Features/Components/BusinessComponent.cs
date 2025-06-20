using Leopotam.EcsLite;

namespace Features.Components
{
    public struct BusinessComponent : IEcsAutoReset<BusinessComponent>
    {
        public int BusinessId;
        public int Level;
        public bool IsOwned;

        public void AutoReset(ref BusinessComponent c)
        {
            c.BusinessId = 0;
            c.Level = 0;
            c.IsOwned = false;
        }
    }
}