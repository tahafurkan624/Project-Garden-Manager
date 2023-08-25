using System.Collections;
using _Main._Scripts.Managers;
using DG.Tweening;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class SprinklerBoostButton : MonoBehaviour
    {
        [SerializeField] private Transform buttonPressPoint;

        private bool isActive = true;
        
        public void OnClick()
        {
            if (!isActive) return;

            isActive = false;
            buttonPressPoint.DOLocalMoveY(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            FarmAreasManager.Instance.BoostSprinkles(this);
        }

        public void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);
        }
    }
}