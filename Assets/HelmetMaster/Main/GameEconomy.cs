using System;
using _Main._Scripts.Utilities;
using HelmetMaster.Main.UI;
using UnityEngine;

namespace HelmetMaster.Main
{
    public class GameEconomy : Singleton<GameEconomy>
    {
        public static int Money;

        public static Action OnMoneyChange;

        private void OnEnable()
        {
            OnMoneyChange += MoneyChanged;
        }

        private void OnDisable()
        {
            OnMoneyChange -= MoneyChanged;
        }

        private void Start()
        {
            SetMoney(GlobalPlayerPrefs.Money);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                AddMoney(200);
            }
        }

        public void AddMoney(int amount, bool animated = true)
        {
            ChangeMoney(amount, animated);
        }

        public void SpendMoney(int amount, bool animated = true)
        {
            ChangeMoney(-amount, animated);
        }

        private void SetMoney(int amount)
        {
            MainCanvas.Instance.SetMoneyText(amount);

            Money = amount;
            OnMoneyChange?.Invoke();
        }

        private void ChangeMoney(int amount, bool animated = true)
        {
            var currentMoney = Money;
            var nextMoney = Money + amount;

            if (animated) MainCanvas.Instance.SetMoneyTextAnimated(currentMoney, nextMoney);
            else MainCanvas.Instance.SetMoneyText(nextMoney);

            Money = nextMoney;
            OnMoneyChange?.Invoke();
        }

        public bool HasEnoughMoney(int amount)
        {
            return Money >= amount;
        }

        private void MoneyChanged()
        {
            // Money Amount Changed
            GlobalPlayerPrefs.Money = Money;
        }
    }
}