using Leopotam.EcsLite;
namespace Features.Events
{
    public struct SaveGameEvent : IEcsAutoReset<SaveGameEvent>
    {
        public void AutoReset(ref SaveGameEvent c)
        {
        }
    }
}