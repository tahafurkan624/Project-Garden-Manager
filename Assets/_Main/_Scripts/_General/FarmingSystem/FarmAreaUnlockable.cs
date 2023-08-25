using System;
using System.Collections;
using System.Collections.Generic;
using HelmetMaster.Main;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class FarmAreaUnlockable : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private int price;
        [SerializeField] private bool startUnlocked;
        [SerializeField] private Collider collider;
        
        public bool IsUnlocked
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt($"Is_Unlocked_{id}", 0));
            set => PlayerPrefs.SetInt($"Is_Unlocked_{id}", (value)? 1:0);
        }

        [SerializeField] private List<GameObject> objectsToOpen = new List<GameObject>();
        [SerializeField] private List<GameObject> objectsToClose = new List<GameObject>();
        [SerializeField] private TMP_Text priceTMP;
        [SerializeField] private FarmAreaRow row;
        private IEnumerator Start()
        {
            priceTMP.text = $"{price.FormatMoney()}";
            yield return null;
            
            if (startUnlocked)
            {
                IsUnlocked = true;
            }
            
            if (IsUnlocked)
            {
                Unlock();
            }
        }

        private void Unlock()
        {
            VibrationManager.Haptic(HapticTypes.LightImpact);

            IsUnlocked = true;
            foreach (var obj in objectsToClose)
            {
                obj.SetActive(false);
            }

            foreach (var obj in objectsToOpen)
            {
                obj.SetActive(true);
            }

            row.UnlockSlots();
            
            collider.enabled = false;
            this.enabled = false;
        }

        public void OnClick()
        {
            if (GameEconomy.Instance.HasEnoughMoney(price))
            {
                GameEconomy.Instance.SpendMoney(price);
                Unlock();
            }
        }
    }
}