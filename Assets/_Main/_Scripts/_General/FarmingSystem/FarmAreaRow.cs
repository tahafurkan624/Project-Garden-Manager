using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class FarmAreaRow : MonoBehaviour
    {
        private List<FarmSlot> slots = new List<FarmSlot>();
        [SerializeField] private Sprinkler sprinkler;
        public FarmArea FarmArea { get; set; }
        public bool IsUnlocked { get; private set; }
        public void AddSlots(FarmSlot slot)
        {
            if (slots.Contains(slot)) return;

            slots.Add(slot);
        }

        public void UnlockSlots()
        {
            IsUnlocked = true;
            
            foreach (var farmSlot in slots)
            {
                farmSlot.IsLocked = false;
            }
            
            FarmArea.OnRowUnlocked();
            sprinkler.Sprinkle();
        }

        public void SetRowSpeed(float speed)
        {
            foreach (var farmSlot in slots)
            {
                farmSlot.SetSpeed(speed);
            }
        }
    }
}