using UnityEngine;

namespace Features.Configs
{
    [CreateAssetMenu(fileName = "BusinessNames", menuName = "Game/Business Names")]
    public class BusinessNamesSO : ScriptableObject
    {
        [Header("Business Names")]
        public BusinessNames[] Names = new BusinessNames[5];

        private void OnValidate()
        {
            if (Names == null || Names.Length != 5)
            {
                var oldData = Names;
                Names = new BusinessNames[5];

                if (oldData != null)
                {
                    for (int i = 0; i < Mathf.Min(oldData.Length, 5); i++)
                    {
                        Names[i] = oldData[i];
                    }
                }
                FillDefaultValues();
            }
        }

        [ContextMenu("Fill Default Values")]
        private void FillDefaultValues()
        {
            Names[0] = new BusinessNames
            {
                BusinessName = "Lemonade Stand",
                Upgrade1Name = "Better Lemons",
                Upgrade2Name = "Ice Machine"
            };

            Names[1] = new BusinessNames
            {
                BusinessName = "Newspaper Delivery",
                Upgrade1Name = "Faster Bike",
                Upgrade2Name = "Multiple Routes"
            };

            Names[2] = new BusinessNames
            {
                BusinessName = "Car Wash",
                Upgrade1Name = "Premium Soap",
                Upgrade2Name = "Automated System"
            };

            Names[3] = new BusinessNames
            {
                BusinessName = "Pizza Restaurant",
                Upgrade1Name = "Wood Oven",
                Upgrade2Name = "Delivery Service"
            };

            Names[4] = new BusinessNames
            {
                BusinessName = "Movie Theater",
                Upgrade1Name = "Digital Projectors",
                Upgrade2Name = "IMAX Screens"
            };
        }
    }
}