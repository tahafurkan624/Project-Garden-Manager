using System;
using System.Collections;
using _Main._Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts._General
{
    public class ScrollButton : MonoBehaviour
    {
        private Button _button;
        private Button Button => _button ??= GetComponent<Button>();

        [SerializeField] private Transform arrow;
        
        private bool onFirstFarm = true;
        private bool isAnimating;
        public bool IsActive { get; private set; }
        private void Start()
        {
            Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (isAnimating) return;

            isAnimating = true;
            StartCoroutine(DelayForAnimation(onFirstFarm));
            
            if (onFirstFarm)
            {
                onFirstFarm = false;
                CameraManager.Instance.ScrollFarmCamUp();
            }
            else
            {
                onFirstFarm = true;
                CameraManager.Instance.ScrollFarmCamDown();
            }
        }

        IEnumerator DelayForAnimation(bool rotate)
        {
            var rot = arrow.eulerAngles;
            rot.z = (rotate) ? -180f : 0f;
            arrow.DORotate(rot, .5f);
            yield return new WaitForSeconds(1f);
            isAnimating = false;
        }

        public void ChangeInteractability(bool isActive)
        {
            gameObject.SetActive(isActive);
            IsActive = isActive;
        }
    }
}