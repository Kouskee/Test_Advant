using UnityEngine;
using System;

namespace Features.Configs
{
    [Serializable]
    public class BusinessData
    {
        [Header("Basic Settings")]
        public float IncomeDelay = 3f;
        public float BaseCost = 3f;
        public float BaseIncome = 3f;

        [Header("Upgrade 1")]
        public float Upgrade1Cost = 50f;
        [Range(0f, 10f)]
        public float Upgrade1Multiplier = 0.5f;

        [Header("Upgrade 2")]
        public float Upgrade2Cost = 400f;
        [Range(0f, 10f)]
        public float Upgrade2Multiplier = 1f;
    }
}