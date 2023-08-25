using System.Collections;
using System.Collections.Generic;
using _Main._Scripts.Managers;
using DG.Tweening;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class Product : MonoBehaviour
    {
        [SerializeField] private float crateScale = 1f;
        
        public void Collect(CreateSlot crate, List<FarmSlot> slots)
        {
            StartCoroutine(MoveToCrate(crate, slots));
        }

        private IEnumerator MoveToCrate(CreateSlot crate, List<FarmSlot> slots)
        {
            transform.SetParent(crate.transform);
            DragArea.Instance.AdjustDragParentPos(crate.DragObject);

            var target = crate.Positions[crate.ProductCount++];
            transform.DOScale(crateScale, 1f);
            transform.DORotate(target.eulerAngles, 1f);
            transform.DOLocalJump(target.localPosition, 4, 1, 1f).OnComplete(() =>
            {
                crate.AddProduct(this);
            });

            yield return new WaitForSeconds(0.2f);
            ClearOccupies(slots);
        }
        
        public void ClearOccupies(List<FarmSlot> slots)
        {
            foreach (var slot in slots)
            {
                if (slot.AnimalAttackingMe)
                {
                    AnimalManager.Instance.FarmCollected();
                    slot.AnimalAttackingMe = false;
                }
                slot.IsOccupied = false;
                slot.currentSeed = null;
            }
        }
    }
}