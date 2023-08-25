using System.Collections;
using System.Collections.Generic;
using _Main._Scripts._General.FarmingSystem;
using DG.Tweening;
using HelmetMaster.Main;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

namespace _Main._Scripts._General
{
    public class CreateSlot : MonoBehaviour
    {
        public bool HasSeed => seedBags.Count > 0;
        public int Revenue, XP;

        [SerializeField] private TMP_Text priceTMPText;

        // public DragArea DragArea { get; set; }
        public ProductTypes type;
        public int maxProductCount = 6;
        public List<SeedBag> seedBags = new List<SeedBag>();
        private List<Product> products = new List<Product>(); 
        public int ProductCount;
        private DragObject _dragObject;
        public DragObject DragObject => _dragObject ??= GetComponent<DragObject>();

        public List<Transform> Positions = new List<Transform>();
        //[SerializeField] private List<Seed> seedPrefabs = new List<Seed>();
        private bool isHapticPlaying;
        public bool IsHapticPlaying => isHapticPlaying;
        private void OnEnable()
        {
            priceTMPText.text = Revenue.FormatMoney();
        }

        private void Start()
        {
            // var randomSeed = seedPrefabs.RandomItem();
            // foreach (var seedBag in seedBags)
            // {
            //     seedBag.OnCrateSpawned(this, randomSeed);
            // }
        }

        public SeedBag NextSeedBag()
        {
            if (seedBags.Count < 1)
            {
                return null;
            }
            return seedBags[0];
        }
        
        public SeedBag GetNextSeedBag()
        {
            var seedBag = seedBags[0];
            seedBags.RemoveAt(0);
            return seedBag;
        }

        public void CantPlaceThere()
        {
            // vibrate and shake crate
            if (isHapticPlaying) return;

            isHapticPlaying = true;
            VibrationManager.Haptic(HapticTypes.Failure);
            StartCoroutine(HapticDelay());
        }

        IEnumerator HapticDelay()
        {
            transform.DOComplete();
            var rotation = transform.localRotation.eulerAngles;
            rotation.y -= 3f;
            transform.localRotation = Quaternion.Euler(rotation);
            rotation.y += 6f;
            transform.DOLocalRotate(rotation, 0.125f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                rotation.y -= 3f;
                transform.localRotation = Quaternion.Euler(rotation);
            });
            yield return new WaitForSeconds(0.4f);
            isHapticPlaying = false;
        }

        public void AddProduct(Product product)
        {
            if (products.Contains(product)) return;

            products.Add(product);
            if (products.Count >= maxProductCount)
            {
                DragArea.Instance.OnCrateSend(DragObject);
                //Destroy(gameObject);
            }
        }

        public void DisablePriceText()
        {
            priceTMPText.gameObject.SetActive(false);
        }
        
        public enum ProductTypes
        {
            Carrot,
            Tomato,
            Eggplant,
            Pepper,
            Watermelon,
            Pumpkin,
            Cabbage
        }
    }
}
