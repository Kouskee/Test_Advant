using Features.Views;
using Leopotam.EcsLite;

namespace Features.Components
{
    public struct BusinessUIComponent : IEcsAutoReset<BusinessUIComponent>
    {
        public BusinessView View;

        public void AutoReset(ref BusinessUIComponent c)
        {
            c.View = null;
        }
    }
}