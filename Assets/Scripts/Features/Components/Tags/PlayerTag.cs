using Leopotam.EcsLite;

namespace Features.Tags
{
    public struct PlayerTag : IEcsAutoReset<PlayerTag>
    {
        public void AutoReset(ref PlayerTag c)
        {
        }
    }
}