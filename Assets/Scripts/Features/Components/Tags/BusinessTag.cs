using Leopotam.EcsLite;

namespace Features.Tags
{
    public struct BusinessTag : IEcsAutoReset<BusinessTag>
    {
        public void AutoReset(ref BusinessTag c)
        {
        }
    }
}