using Leopotam.EcsLite;

namespace Features.Components
{
    public struct IncomeComponent : IEcsAutoReset<IncomeComponent>
    {
        public float BaseIncome;
        public float CurrentIncome;
        public float IncomeDelay;
        public float Progress;
        public float LastUpdateTime;

        public void AutoReset(ref IncomeComponent c)
        {
            c.BaseIncome = 0f;
            c.CurrentIncome = 0f;
            c.IncomeDelay = 0f;
            c.Progress = 0f;
            c.LastUpdateTime = 0f;
        }
    }
}