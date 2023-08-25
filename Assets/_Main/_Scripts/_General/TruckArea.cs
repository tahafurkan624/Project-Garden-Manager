using System.Collections;
using System.Collections.Generic;
using _Main._Scripts.Managers;
using DG.Tweening;
using HelmetMaster.Main;
using HelmetMaster.Main.UI;
using MoreMountains.NiceVibrations;
using PathCreation.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts._General
{
    public class TruckArea : MonoBehaviour
    {
        [Header("Unlock")] [SerializeField]
        public bool IsUnlocked;
        public int Price;
        [SerializeField] private TMP_Text priceTmpText;
        public string SaveID;
        public bool isBigTruck;
        [Header("Attributes & Components")]
        public bool ReadyToSell;

        [SerializeField] private List<Transform> SellRoute = new List<Transform>();

        [SerializeField] private GameObject moneyObj;
        [SerializeField] private Transform truckTransform, carryTransform;
        private Transform _truckBodyTransform;
        [SerializeField] private PathFollower pf;
        [SerializeField] private Image fillImage;
        [SerializeField] private ParticleSystem landingParticle;
        [SerializeField] private float truckMoneyMultiplier;

        private CreateSlot _loadedCrate;
        private float truckSpeed;
        public bool IsLoaded { get; private set; }

        private void Awake()
        {
            if (PlayerPrefs.GetInt(SaveID) == 1 && SaveID != "Truck_01")
            {
                Unlock(true);
            }
            else if (SaveID == "Truck_00")
            {
                Unlock(true);
            }
            else
            {
                priceTmpText.text = Price.FormatMoney();
            }
            _truckBodyTransform = truckTransform.GetChild(0);
            truckSpeed = pf.speed;
        }

        public void CollectMoney(bool FtueActive = false)
        {
            if (FtueActive) FtueManager.Instance.MoneyCollected();
            ReadyToSell = false;
            GameEconomy.Instance.AddMoney((int)(_loadedCrate.Revenue * truckMoneyMultiplier));
            LevelProgress.Instance.EarnXp(_loadedCrate.XP);
            var moneyTrans = MoneyTextPooler.Instance.GenerateFloatingMoney(((int)(_loadedCrate.Revenue * truckMoneyMultiplier)).FormatMoney(), carryTransform.position, 4f, 1f);
            var cam = CameraManager.Instance.sellCam.transform;
            var rot = Quaternion.LookRotation((moneyTrans.position - cam.position), cam.up);
            moneyTrans.rotation = rot;
            moneyObj.SetActive(false);

            _loadedCrate = null;
            MainCanvas.Instance.changeSceneButton.MoneyCollected();
        }

        public void LoadCrateToTruck(CreateSlot loadedCrate, bool FtueActive = false)
        {
            VibrationManager.Haptic(HapticTypes.LightImpact);

            loadedCrate.DisablePriceText();
            _loadedCrate = loadedCrate;
            IsLoaded = true;
            DragArea.Instance.OnCrateShipped(FtueActive);
            _loadedCrate.transform.SetParent(carryTransform);
            _loadedCrate.transform.DOScale(Vector3.one * 0.4f, .6f);
            _loadedCrate.transform.DOJump(CameraManager.Instance.GetDragSellCamPos(carryTransform.position), 2, 1, .8f).OnComplete(()=>
            {
                _loadedCrate.transform.position = carryTransform.position;
                CarLeave();
            });

            // landingParticle.Play();

            StartCoroutine(SellCoroutine(FtueActive));
            if(FtueActive) FtueManager.Instance.CheckTappedShipFtueCompleted();
        }

        public void CantPlaceThatCrate()
        {
            _truckBodyTransform.DOComplete();
            var rotation = _truckBodyTransform.localRotation.eulerAngles;
            rotation.y -= 3f;
            _truckBodyTransform.localRotation = Quaternion.Euler(rotation);
            rotation.y += 6f;
            _truckBodyTransform.DOLocalRotate(rotation, 0.125f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                rotation.y -= 3f;
                _truckBodyTransform.localRotation = Quaternion.Euler(rotation);
            });
        }
        
        private void CarLeave()
        {
            pf.GoingReverse = false;
            pf.IsActive = true;
            pf.speed = 0;
            DOTween.To(() => pf.speed, x => pf.speed = x, truckSpeed, 2f)
                .SetEase(Ease.InOutQuad);
            _truckBodyTransform.DOLocalMoveY(.12f, .2f).SetEase(Ease.OutBack).SetLoops(-1 ,LoopType.Yoyo);
            _truckBodyTransform.DOLocalRotate(-Vector3.forward * 2.4f, 0);
            _truckBodyTransform.DOLocalRotate(Vector3.forward * 2.4f, .36f).SetEase(Ease.Linear).SetLoops(-1 ,LoopType.Yoyo);

            // if (isBigTruck)
            // {
            //     for (int i = 0; i < truckTransform.GetChild(1).GetChild(0).childCount; i++)
            //     {
            //         truckTransform.GetChild(1).GetChild(0).GetChild(i).DOLocalRotate(Vector3.right * 180, .36f)
            //             .SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            //     }    
            // }
            
            // yield break;
            //
            // yield return new WaitForSeconds(7.5f);
            //
            // StartCoroutine(CarReturn());
        }

        private void CarReturn()
        {
            pf.FixDistanceTravelled();
            pf.IsActive = true;
            pf.GoingReverse = true;
            pf.speed = 0;
            DOTween.To(() => pf.speed, x => pf.speed = x, truckSpeed, 2f)
                .SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    IsLoaded = false;
                    ReadyToSell = true;
                    MainCanvas.Instance.changeSceneButton.SomethingSold();
                });
            // _truckBodyTransform.DOLocalMoveY(.12f, .2f).SetEase(Ease.OutBack).SetLoops(-1 ,LoopType.Yoyo);
            // _truckBodyTransform.DOLocalRotate(-Vector3.forward * 2.4f, 0);
            // _truckBodyTransform.DOLocalRotate(Vector3.forward * 2.4f, .36f).SetEase(Ease.Linear).SetLoops(-1 ,LoopType.Yoyo);
            // for (int i = 0; i < truckTransform.GetChild(1).GetChild(0).childCount; i++)
            // {
            //     truckTransform.GetChild(1).GetChild(0).GetChild(i).DOLocalRotate(Vector3.right * 180, .36f)
            //         .SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            // }

            // yield break;
        }

        private IEnumerator SellCoroutine(bool FtueActive = false)
        {
            float elapsedTime = 0f;
            float duration = 30f - GlobalPlayerPrefs.TimeDiscount;
            if (FtueActive) duration = 10;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > duration) elapsedTime = duration;
                fillImage.fillAmount = elapsedTime / duration;
                yield return null;
            }

            fillImage.fillAmount = 0;
            var crate = _loadedCrate.gameObject;
            Destroy(crate);
            moneyObj.SetActive(true);

            CarReturn();
            if (FtueActive) StartCoroutine(CollectMoneyRoutine());

            IEnumerator CollectMoneyRoutine()
            {
                yield return new WaitForSeconds(2f);
                FtueManager.Instance.MoneyReadyToCollect();
            }
        }

        public void Returned()
        {
            _truckBodyTransform.DOKill();
            // if (isBigTruck)
            // {
            //     for (int i = 0; i < truckTransform.GetChild(1).GetChild(0).childCount; i++)
            //     {
            //         truckTransform.GetChild(1).GetChild(0).GetChild(i).DOKill();
            //     }    
            // }
        }

        public void Unlock(bool load = false)
        {
            if (load)
            {
                
            }
            else
            {
                PlayerPrefs.SetInt(SaveID, 1);
            }
            if(isBigTruck) DragArea.Instance.BigSeedsEnabled = true;
            
            VibrationManager.Haptic(HapticTypes.LightImpact);

            IsUnlocked = true;
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}