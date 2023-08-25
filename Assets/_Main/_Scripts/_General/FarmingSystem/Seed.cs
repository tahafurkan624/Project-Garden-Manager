using System.Collections;
using System.Collections.Generic;
using _Main._Scripts.Managers;
using DG.Tweening;
using HelmetMaster.Main;
using HelmetMaster.Main.UI;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts._General.FarmingSystem
{
    public class Seed : MonoBehaviour
    {
        public Vector3 size;

        public CreateSlot Crate;
        public bool IsGrown { get; private set; }

        [HideInInspector] public List<FarmSlot> plantedSlots = new List<FarmSlot>();
        [SerializeField] private Product productPrefab;
        [SerializeField] private float growDuration = 20f;
        [SerializeField] private GameObject timerGo;
        [SerializeField] private Image timerFill;
        [SerializeField] private Animator animator;
        
        private ParticleSystem grownParticle;
        private float elapsedTime = 0;
        public Transform Ground { get; set; }
        // public void OnCrateSpawned(CreateSlot slot)
        // {
        //     Crate = slot;
        // }
        
        public void Grow()
        {
            elapsedTime = 0;
            DOTween.To(() => elapsedTime, x => elapsedTime = x, 1f, growDuration).SetId("RoundUpdate")
                .SetEase(Ease.Linear).OnUpdate(() => { timerFill.fillAmount = elapsedTime; }).OnComplete(() =>
                {
                    timerGo.transform.DOScale(0, 0.2f).OnComplete(() =>
                    {
                        timerGo.gameObject.SetActive(false);
                        timerGo.transform.localScale = Vector3.one;
                    });
                });
        }

        public void GrowFtue()
        {
            elapsedTime = 0;
            DOTween.To(() => elapsedTime, x => elapsedTime = x, 1f, growDuration).SetId("RoundUpdate")
                .SetEase(Ease.Linear).OnUpdate(() => { timerFill.fillAmount = elapsedTime; }).OnComplete(() =>
                {
                    timerGo.transform.DOScale(0, 0.2f).OnComplete(() =>
                    {
                        timerGo.gameObject.SetActive(false);
                        timerGo.transform.localScale = Vector3.one;
                        FtueManager.Instance.AddGrewSeed();
                    });
                });
        }
        
        public void OnGrown()
        {
            IsGrown = true;
            grownParticle = ParticleManager.Instance.PlayParticle(ParticleTag.Grown, transform.position);
            var main = grownParticle.main;
            main.startDelay = 1f;
            
            MainCanvas.Instance.changeSceneButton.SomethingGrown();
        }
        
        public void Collect()
        {
            VibrationManager.Haptic(HapticTypes.LightImpact);
            IsGrown = false;
            Destroy(Ground.gameObject);
            var pos = transform.position + Vector3.up;
            pos = CameraManager.Instance.GetDragCamPos(pos);
            var grownProduct = Instantiate(productPrefab.transform, pos, Quaternion.identity).GetComponent<Product>();

            grownParticle.gameObject.SetActive(false);
            grownProduct.Collect(Crate, plantedSlots);
            gameObject.SetActive(false);
        }

        public void Steal(Transform animalHand)
        {
            VibrationManager.Haptic(HapticTypes.Warning);
            IsGrown = false;
            Destroy(Ground.gameObject);
            
            Instantiate(productPrefab.transform, animalHand.position, Quaternion.identity, animalHand);
            
            foreach (var slot in plantedSlots)
            {
                slot.IsOccupied = false;
                slot.currentSeed = null;
            }

            if (grownParticle != null)
            {
                grownParticle.gameObject.SetActive(false);
            }

            Crate.Revenue -= (Crate.Revenue / Crate.maxProductCount);
            Crate.maxProductCount--;
            MainCanvas.Instance.changeSceneButton.SomethingGotStolen();
            gameObject.SetActive(false);
        }
        
        public void SetSpeed(float speed)
        {
            animator.speed = speed;
        }
    }
}