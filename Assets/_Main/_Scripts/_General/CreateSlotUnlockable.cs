using System;
using HelmetMaster.Main;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

namespace _Main._Scripts._General
{
    public class CreateSlotUnlockable : MonoBehaviour
    {
        public bool HasBought;
        public int Price;
        private int[] _prices = new[] {50, 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000};

        public Material Mat;

        [SerializeField] private TMP_Text priceTMPText;

        private DragObject _dragObject;
        public DragObject DragObject { get { if (_dragObject == null) _dragObject = GetComponent<DragObject>(); return _dragObject; } }

        private int TotalCrate => Mathf.Max(GlobalPlayerPrefs.SavedItems.TotalCreateAmount, 0);
        private void OnEnable()
        {
            var index = Mathf.Max(TotalCrate - 3, 0);
            index = Mathf.Min(index, _prices.Length - 1);
            priceTMPText.text = _prices[index].FormatMoney();
            Price = _prices[index];
        }

        public void Unlock()
        {
            VibrationManager.Haptic(HapticTypes.LightImpact);

            DragArea.Instance.UnlockableCrateAvailable = false;
            DragArea.Instance.UnlockableDragObj = null;
            GlobalPlayerPrefs.SavedItems.TotalCreateAmount++;
            HasBought = true;

            DragArea.Instance.RemoveDragObject(DragObject);
            DragArea.Instance.AddCrateSlot();
            DragArea.Instance.AdjustCreatePositions();
            DragArea.Instance.CheckUnlockableCrate();
            Destroy(gameObject);
            // Idırı Vıdırı
        }
    }
}