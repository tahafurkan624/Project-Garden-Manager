using System.Collections;
using _Main._Scripts.Managers;
using DG.Tweening;
using HelmetMaster.Main;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class SeedBag : MonoBehaviour
    {
        public Seed Seed;
        [SerializeField] private Transform ground;
        [SerializeField] private Vector3 groundOffset;
        [SerializeField] private Vector3 seedOffset;
        
        public void OnCrateSpawned(CreateSlot slot, Seed seedPrefab)
        {
            // Seed = Instantiate(seedPrefab, transform.position, Quaternion.identity);
            // Seed.gameObject.SetActive(false);
            // Seed.OnCrateSpawned(slot);
        }
        
        public void Plant(Vector3 target, bool growFtue = false)
        {
            VibrationManager.Haptic(HapticTypes.LightImpact);

            StartCoroutine(MoveToTarget(target, growFtue));
        }
        
        private IEnumerator MoveToTarget(Vector3 target, bool growFtue)
        {
            transform.SetParent(null);
            transform.position = target + new Vector3(-1.5f,1,0) + seedOffset;

            var seedTarget = target + seedOffset;
            
            Seed.transform.SetParent(null);
            Seed.transform.position = seedTarget;
            ParticleManager.Instance.PlayParticle(ParticleTag.Growing, seedTarget);

            // Dropping seeds animation
            ground.position = target + groundOffset;
            ground.SetParent(null);
            ground.localScale = Vector3.zero;
            ground.gameObject.SetActive(true);
            ground.DOScale(1f, 0.3f);
            
            Seed.Ground = ground;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -40f));

            Sequence seedBagSequence = DOTween.Sequence();
            
            seedBagSequence.Append(transform.DORotate(new Vector3(0f, 0f, -85f), 0.5f).SetEase(Ease.OutQuad));
            seedBagSequence.Append(transform.DOShakeRotation(0.5f, new Vector3(0f, 0f, 5f), 5, 30f).SetEase(Ease.Linear).OnStart(
                () =>
                {
                    ParticleManager.Instance.PlayParticle(ParticleTag.SeedBombing, transform.position + (transform.up * 1.5f), Quaternion.Euler(new Vector3(-90,0,0)));
                }));
            seedBagSequence.Append(transform.DORotate(new Vector3(0f, 0f, -40f), 0.5f).SetEase(Ease.OutQuad));
            yield return seedBagSequence.Play().WaitForCompletion();
            
            Seed.gameObject.SetActive(true);
            if (growFtue) Seed.GrowFtue();
            else Seed.Grow();
            gameObject.SetActive(false);
        }
    }
}