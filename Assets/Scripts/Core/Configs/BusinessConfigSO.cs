using UnityEngine;

namespace Features.Configs
{
    [CreateAssetMenu(fileName = "BusinessConfig", menuName = "Game/Business Config")]
    public class BusinessConfigSO : ScriptableObject
    {
        [Header("Business Configuration")]
        public BusinessData[] Businesses = new BusinessData[5];

        private void OnValidate()
        {
            if (Businesses == null || Businesses.Length != 5)
            {
                var oldData = Businesses;
                Businesses = new BusinessData[5];

                if (oldData != null)
                {
                    for (int i = 0; i < Mathf.Min(oldData.Length, 5); i++)
                    {
                        Businesses[i] = oldData[i];
                    }
                }
                //FillDefaultValues();
            }
        }

        [ContextMenu("Fill Default Values")]
        private void FillDefaultValues()
        {
            Businesses[0] = new BusinessData
            {
                IncomeDelay = 3f,
                BaseCost = 3f,
                BaseIncome = 3f,
                Upgrade1Cost = 50f,
                Upgrade1Multiplier = 0.5f,
                Upgrade2Cost = 400f,
                Upgrade2Multiplier = 1f
            };

            Businesses[1] = new BusinessData
            {
                IncomeDelay = 6f,
                BaseCost = 40f,
                BaseIncome = 40f,
                Upgrade1Cost = 1200f,
                Upgrade1Multiplier = 1f,
                Upgrade2Cost = 4000f,
                Upgrade2Multiplier = 2f
            };

            Businesses[2] = new BusinessData
            {
                IncomeDelay = 10f,
                BaseCost = 200f,
                BaseIncome = 200f,
                Upgrade1Cost = 6000f,
                Upgrade1Multiplier = 1f,
                Upgrade2Cost = 20000f,
                Upgrade2Multiplier = 1.5f
            };

            Businesses[3] = new BusinessData
            {
                IncomeDelay = 17f,
                BaseCost = 1000f,
                BaseIncome = 1000f,
                Upgrade1Cost = 15000f,
                Upgrade1Multiplier = 1f,
                Upgrade2Cost = 50000f,
                Upgrade2Multiplier = 2f
            };

            Businesses[4] = new BusinessData
            {
                IncomeDelay = 30f,
                BaseCost = 5000f,
                BaseIncome = 5000f,
                Upgrade1Cost = 100000f,
                Upgrade1Multiplier = 2f,
                Upgrade2Cost = 500000f,
                Upgrade2Multiplier = 4f
            };
        }
    }
}