using System.Collections.Generic;
using _Main._Scripts.Utilities;
using UnityEngine;

namespace HelmetMaster.Main
{
    public class MoneyTextPooler : Singleton<MoneyTextPooler>
    {
        [SerializeField] private FloatingMoneyText moneyTextPrefab;
        [SerializeField] private int poolAmount;

        private readonly List<FloatingMoneyText> floatingMoneys = new List<FloatingMoneyText>();

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < poolAmount; i++)
            {
                var money = Instantiate(moneyTextPrefab, transform);
                money.Init();
                floatingMoneys.Add(money);
            }
        }

        public Transform GenerateFloatingMoney(string textString, Vector3 pos, float YMove, float duration)
        {
            var money = PullFromPool();
            money.PlayFloatingMoney(textString, pos, YMove, duration);
            return money.transform;
        }

        private int lastIndex;
        private FloatingMoneyText PullFromPool()
        {
            var idx = lastIndex % poolAmount;
            lastIndex++;
            floatingMoneys[idx].gameObject.SetActive(true);
            return floatingMoneys[idx];
        }
    }
}