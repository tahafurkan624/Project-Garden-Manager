using System;
using System.Collections;
using _Main._Scripts._General;
using _Main._Scripts._General.FarmingSystem;
using _Main._Scripts.Utilities;
using HelmetMaster.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main._Scripts.Managers
{
    public class FtueManager : Singleton<FtueManager>
    {
        [SerializeField] private Transform FtueHand, FtueHand3D;
        [SerializeField] private GameObject SelectSeedFtueGo, DragToPlantSeedFtueGo, WaitSeedGrowGo, DragToCollectGo, 
            TapToSellAreaGo, SelectProductFtueGo, TapToShipProductFtueGo, WaitTruckSellGo, CollectMoneyGo, 
            BuyCargoCarGo, TapToFarmGo, holdToBuyCrateGo;
        [SerializeField] private Button SellAreaButton;
        [SerializeField] private TruckArea cargoCar, cargoCar2;
        [SerializeField] private FarmArea farmArea;

        private Animator FtueHandAnimator;
        [SerializeField] private Animator FtueHandAnimator3D;

        public bool SelectSeedFtuePlayed
        {
            get => PlayerPrefs.GetInt("SelectFtue", 0) == 1;
            set => PlayerPrefs.SetInt("SelectFtue", value ? -1 : 0);
        }
        public bool DragToPlantSeedFtuePlayed
        {
            get => PlayerPrefs.GetInt("DragToPlantSeed", 0) == 1;
            set => PlayerPrefs.SetInt("DragToPlantSeed", value ? -1 : 0);
        }
        public bool WaitSeedGrowFtuePlayed
        {
            get => PlayerPrefs.GetInt("WaitSeedGrow", 0) == 1;
            set => PlayerPrefs.SetInt("WaitSeedGrow", value ? -1 : 0);
        }
        public bool DragToCollectFtuePlayed
        {
            get => PlayerPrefs.GetInt("DragToCollect", 0) == 1;
            set => PlayerPrefs.SetInt("DragToCollect", value ? -1 : 0);
        }
        public bool TapToSellAreaButtonFtuePlayed
        {
            get => PlayerPrefs.GetInt("TapToSellAreaButton", 0) == 1;
            set => PlayerPrefs.SetInt("TapToSellAreaButton", value ? -1 : 0);
        }
        public bool TapToSelectProductFtuePlayed
        {
            get => PlayerPrefs.GetInt("TapToSelectProduct", 0) == 1;
            set => PlayerPrefs.SetInt("TapToSelectProduct", value ? -1 : 0);
        }
        public bool TapToShipProductFtuePlayed
        {
            get => PlayerPrefs.GetInt("TapToShipProduct", 0) == 1;
            set => PlayerPrefs.SetInt("TapToShipProduct", value ? -1 : 0);
        }
        public bool WaitTruckSellFtuePlayed
        {
            get => PlayerPrefs.GetInt("WaitTruckSell", 0) == 1;
            set => PlayerPrefs.SetInt("WaitTruckSell", value ? -1 : 0);
        }
        public bool CollectMoneyFtuePlayed
        {
            get => PlayerPrefs.GetInt("CollectMoney", 0) == 1;
            set => PlayerPrefs.SetInt("CollectMoney", value ? -1 : 0);
        }
        public bool BuyCargoCarFtuePlayed
        {
            get => PlayerPrefs.GetInt("BuyCargoCar", 0) == 1;
            set => PlayerPrefs.SetInt("BuyCargoCar", value ? -1 : 0);
        }
        public bool TapToGoFarmButtonFtuePlayed
        {
            get => PlayerPrefs.GetInt("TapToGoFarm", 0) == 1;
            set => PlayerPrefs.SetInt("TapToGoFarm", value ? -1 : 0);
        }
        public bool HoldToBuyCrateFtuePlayed
        {
            get => PlayerPrefs.GetInt("HoldToBuyCrate", 0) == 1;
            set => PlayerPrefs.SetInt("HoldToBuyCrate", value ? -1 : 0);
        }
        public bool FirstFtueDone
        {
            get => PlayerPrefs.GetInt("FirstFtueDone", 0) == 1;
            set => PlayerPrefs.SetInt("FirstFtueDone", value ? 1 : 0);
        }

        private int _grewPlantCount, _collectedPlantCount;

        public static bool FtueActive;
        public bool SeedClickedFtueActive, DragPlantSeedFtueActive, WaitSeedGrowFtueActive, DragToCollectFtueActive,
            TapToSellAreaFtueActive, TapToSelectFtueActive, TapToShipFtueActive, WaitTruckSellFtueActive, 
            CollectMoneyFtueActive, BuyCargoCarFtueActive, TapToFarmAreaFtueActive, HoldToBuyCrateFtueActive;
        private bool _seedFtueCompleted, _dragToPlantCompleted, _waitSeedGrowCompleted, _dragToCollectCompleted, 
            _tapToSellAreaCompleted, _selectProductFtueCompleted, _tapToShipFtueCompleted, _waitTruckSellCompleted, 
            _moneyCollectCompleted, _tapToBuyCargoCarCompleted, _tapToFarmAreaCompleted, _holdToBuyCrateCompleted;
        public int DragToPlantCount;

        private Coroutine _dragToPlantRoutine, _dragToCollectRoutine;
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int PressIdle = Animator.StringToHash("PressIdle");
        private static readonly int ClickIdle = Animator.StringToHash("ClickIdle");

        protected override void Awake()
        {
            base.Awake();
            FtueHandAnimator = FtueHand.GetComponent<Animator>();
        }

        private IEnumerator Start()
        {
            if (FirstFtueDone)
            {
                FtueActive = false;
                PlayerPrefs.SetInt("Truck_01", 1);
                cargoCar2.Unlock();
                Destroy(gameObject);
                yield break;
            }

            if(!TapToSellAreaButtonFtuePlayed) SellAreaButton.gameObject.SetActive(false);
            GameEconomy.Instance.SpendMoney(GlobalPlayerPrefs.Money);

            DragArea.IsActive = false;
            DragAreaSell.IsActive = false;
            InputHandler.IsActive = false;
            if (SelectSeedFtuePlayed)
            {
                Destroy(SelectSeedFtueGo);
            }
            else
            {
                FtueActive = true;
                SelectSeedFtueGo.SetActive(true);
                InputHandler.IsActive = false;
                SeedClickedFtueActive = true;
                var pos = CameraManager.Instance.dragCam.WorldToScreenPoint(DragArea.Instance.SelectedCreateSlot.transform.position); 
                FtueHand.gameObject.SetActive(true);
                FtueHandAnimator.SetTrigger(ClickIdle);
                FtueHand.position = pos;

                yield return new WaitUntil(()=> _seedFtueCompleted);

                Destroy(SelectSeedFtueGo);
                SeedClickedFtueActive = false;
            }


            if (DragToPlantSeedFtuePlayed)
            {
                Destroy(DragToPlantSeedFtueGo);
            }
            else
            {
                FtueActive = true;
                DragToPlantSeedFtueGo.SetActive(true);
                FtueHand.gameObject.SetActive(true);
                InputHandler.IsActive = false;
                DragPlantSeedFtueActive = true;
                _dragToPlantRoutine = StartCoroutine(DragToPlantRoutine());

                yield return new WaitUntil(()=> _dragToPlantCompleted);

                StopCoroutine(_dragToPlantRoutine);
                Destroy(DragToPlantSeedFtueGo);
                DragPlantSeedFtueActive = false;
            }

            if (WaitSeedGrowFtuePlayed)
            {
                Destroy(WaitSeedGrowGo);
            }
            else
            {
                FtueActive = true;
                WaitSeedGrowGo.SetActive(true);
                InputHandler.IsActive = false;
                WaitSeedGrowFtueActive = true;      // Useless
                Time.timeScale = 5;

                yield return new WaitUntil(()=> _waitSeedGrowCompleted);

                Destroy(WaitSeedGrowGo);
                WaitSeedGrowFtueActive = false;      // Useless
                WaitSeedGrowFtuePlayed = true;
                Time.timeScale = 1;
            }

            if (DragToCollectFtuePlayed)
            {
                Destroy(DragToCollectGo);
            }
            else
            {
                FtueActive = true;
                DragToCollectGo.SetActive(true);
                FtueHand.gameObject.SetActive(true);
                InputHandler.IsActive = false;
                DragToCollectFtueActive = true;
                var posA = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[0,0].Pos); 
                var posB = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[farmArea.Slots.GetLength(0)-1,0].Pos);
                _dragToCollectRoutine = StartCoroutine(DragToCollectRoutine());

                yield return new WaitUntil(()=> _dragToCollectCompleted);

                StopCoroutine(_dragToCollectRoutine);
                Destroy(DragToCollectGo);
                DragToCollectFtueActive = false;

                yield return new WaitForSeconds(1.25f);
            }

            if (TapToSellAreaButtonFtuePlayed)
            {
                SellAreaButton.gameObject.SetActive(true);
                Destroy(TapToSellAreaGo);
            }
            else
            {
                FtueActive = true;
                TapToSellAreaGo.SetActive(true);
                SellAreaButton.gameObject.SetActive(true);
                SellAreaButton.onClick.AddListener(SellAreaTappedFtue);
                FtueHand.gameObject.SetActive(true);
                TapToSellAreaFtueActive = true;      // Useless
                FtueHand.position = SellAreaButton.transform.position + Vector3.right * -80;
                FtueHandAnimator.SetTrigger(ClickIdle);
                InputHandler.IsActive = false;

                yield return new WaitUntil(()=> _tapToSellAreaCompleted);

                SellAreaButton.gameObject.SetActive(false);
                Destroy(TapToSellAreaGo);
                TapToSellAreaFtueActive = false;      // Useless
                FtueHand.gameObject.SetActive(false);
            }

            if (TapToSelectProductFtuePlayed)
            {
                Destroy(SelectProductFtueGo);
            }
            else
            {
                FtueActive = true;
                SelectProductFtueGo.SetActive(true);
                InputHandler.IsActive = false;
                TapToSelectFtueActive = true;
                var pos = CameraManager.Instance.dragSellCam.WorldToScreenPoint(DragAreaSell.Instance.SelectedCreateSlot.transform.position) + Vector3.up * 80; 
                FtueHand.gameObject.SetActive(true);
                FtueHandAnimator.SetTrigger(ClickIdle);
                FtueHand.position = pos;

                yield return new WaitUntil(()=> _selectProductFtueCompleted);

                Destroy(SelectProductFtueGo);
                TapToSelectFtueActive = false;
            }

            if (TapToShipProductFtuePlayed)
            {
                Destroy(TapToShipProductFtueGo);
            }
            else
            {
                FtueActive = true;
                TapToShipProductFtueGo.SetActive(true);
                InputHandler.IsActive = false;
                TapToShipFtueActive = true;
                var pos = CameraManager.Instance.sellCam.WorldToScreenPoint(cargoCar.transform.position);
                FtueHand.gameObject.SetActive(true);
                FtueHandAnimator.SetTrigger(ClickIdle);
                FtueHand.position = pos;

                yield return new WaitUntil(()=> _tapToShipFtueCompleted);

                Destroy(TapToShipProductFtueGo);
                TapToShipFtueActive = false;
            }

            if (WaitTruckSellFtuePlayed)
            {
                Destroy(WaitTruckSellGo);
            }
            else
            {
                FtueActive = true;
                WaitTruckSellGo.SetActive(true);
                InputHandler.IsActive = false;
                WaitTruckSellFtueActive = true;      // Useless

                yield return new WaitUntil(()=> _waitTruckSellCompleted);

                Destroy(WaitTruckSellGo);
                WaitTruckSellFtueActive = false;      // Useless
                WaitTruckSellFtuePlayed = true;
            }

            if (CollectMoneyFtuePlayed)
            {
                Destroy(CollectMoneyGo);
            }
            else
            {
                FtueActive = true;
                CollectMoneyGo.SetActive(true);
                InputHandler.IsActive = false;
                CollectMoneyFtueActive = true;
                var pos = CameraManager.Instance.sellCam.WorldToScreenPoint(cargoCar.transform.position) - Vector3.up * 80; 
                FtueHand.gameObject.SetActive(true);
                FtueHandAnimator.SetTrigger(ClickIdle);
                FtueHand.position = pos;

                yield return new WaitUntil(()=> _moneyCollectCompleted);

                FtueHand.gameObject.SetActive(false);
                CollectMoneyFtueActive = false;
                CollectMoneyFtuePlayed = true;
                Destroy(CollectMoneyGo);
            }

            if (BuyCargoCarFtuePlayed)
            {
                Destroy(BuyCargoCarGo);
            }
            else
            {
                FtueActive = true;
                BuyCargoCarGo.SetActive(true);
                InputHandler.IsActive = false;
                BuyCargoCarFtueActive = true;
                var pos = CameraManager.Instance.sellCam.WorldToScreenPoint(cargoCar2.transform.position) - Vector3.up * 80; 
                FtueHand.gameObject.SetActive(true);
                FtueHandAnimator.SetTrigger(ClickIdle);
                FtueHand.position = pos;
            
                yield return new WaitUntil(()=> _tapToBuyCargoCarCompleted);
            
                FtueHand.gameObject.SetActive(false);
                BuyCargoCarFtueActive = false;
                Destroy(BuyCargoCarGo);
            }

            if (TapToGoFarmButtonFtuePlayed)
            {
                SellAreaButton.gameObject.SetActive(true);
                Destroy(TapToFarmGo);
            }
            else
            {
                FtueActive = true;
                TapToFarmGo.SetActive(true);
                SellAreaButton.gameObject.SetActive(true);
                SellAreaButton.onClick.AddListener(FarmAreaTappedFtue);
                FtueHand.gameObject.SetActive(true);
                TapToFarmAreaFtueActive = true;      // Useless
                FtueHand.position = SellAreaButton.transform.position + Vector3.right * -80;
                FtueHandAnimator.SetTrigger(ClickIdle);
                InputHandler.IsActive = false;

                yield return new WaitUntil(()=> _tapToFarmAreaCompleted);

                Destroy(TapToFarmGo);
                TapToFarmAreaFtueActive = false;      // Useless
                FtueHand.gameObject.SetActive(false);
            }

            if (HoldToBuyCrateFtuePlayed)
            {
                Destroy(holdToBuyCrateGo);
            }
            else
            {
                FtueActive = true;
                holdToBuyCrateGo.SetActive(true);
                InputHandler.IsActive = false;
                DragArea.IsActive = false;
                HoldToBuyCrateFtueActive = true;
                var pos = DragArea.Instance.UnlockableDragObj.transform.position + new Vector3(1.0f,2.1f,-4);
                FtueHand3D.gameObject.SetActive(true);
                //FtueHandAnimator3D.SetTrigger(ClickIdle);
                FtueHand3D.position = pos;
                FtueHand3D.transform.SetParent(DragArea.Instance.UnlockableDragObj.transform);
                DragArea.Instance.AdjustDragParentPosForFtue();

                yield return new WaitUntil(()=> _holdToBuyCrateCompleted);

                DragArea.Instance.AdjustCreatePositions();
                FtueHand3D.gameObject.SetActive(false);
                Destroy(holdToBuyCrateGo);
                HoldToBuyCrateFtueActive = false;
            }

            DragArea.IsActive = true;
            DragAreaSell.IsActive = true;
            InputHandler.IsActive = true;
            SellAreaButton.gameObject.SetActive(true);
            FtueActive = false;
            FirstFtueDone = true;
            Destroy(FtueHand3D.gameObject);
            Destroy(gameObject);
        }

        private void SellAreaTappedFtue()
        {
            _tapToSellAreaCompleted = true;
            TapToSellAreaButtonFtuePlayed = true;
            SellAreaButton.onClick.RemoveListener(SellAreaTappedFtue);
        }

        private void FarmAreaTappedFtue()
        {
            _tapToFarmAreaCompleted = true;
            TapToGoFarmButtonFtuePlayed = true;
            SellAreaButton.onClick.RemoveListener(FarmAreaTappedFtue);
            SellAreaButton.gameObject.SetActive(false);
        }

        private IEnumerator DragToPlantRoutine()
        {
            var posA = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[0,1].Pos); 
            var posB = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[4,1].Pos);
            var posC = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[4,0].Pos);
            var posD = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[0,0].Pos);
            while (!_dragToPlantCompleted)
            {
                FtueHandAnimator.SetTrigger(Idle);
                FtueHand.position = posA;
                yield return new WaitForSeconds(.6f);
                FtueHandAnimator.SetTrigger(PressIdle);

                var elapsedTime = 0f;
                var duration = 0.98f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > duration) elapsedTime = duration;
                    FtueHand.position = Vector3.Lerp(posA, posB, elapsedTime / duration);
                    yield return null;
                }

                elapsedTime = 0f;
                duration = .28f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > duration) elapsedTime = duration;
                    FtueHand.position = Vector3.Lerp(posB, posC, elapsedTime / duration);
                    yield return null;
                }

                elapsedTime = 0f;
                duration = 0.98f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > duration) elapsedTime = duration;
                    FtueHand.position = Vector3.Lerp(posC, posD, elapsedTime / duration);
                    yield return null;
                }

                FtueHandAnimator.SetTrigger(Idle);
                yield return new WaitForSeconds(.6f);
            }
        }

        private IEnumerator DragToCollectRoutine()
        {
            var posA = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[0,1].Pos); 
            var posB = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[4,1].Pos);
            var posC = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[4,0].Pos);
            var posD = CameraManager.Instance.farmCam.WorldToScreenPoint(farmArea.Slots[0,0].Pos);

            while (!_dragToCollectCompleted)
            {
                FtueHandAnimator.SetTrigger(Idle);
                FtueHand.position = posA;
                yield return new WaitForSeconds(.6f);
                FtueHandAnimator.SetTrigger(PressIdle);

                var elapsedTime = 0f;
                var duration = 0.98f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > duration) elapsedTime = duration;
                    FtueHand.position = Vector3.Lerp(posA, posB, elapsedTime / duration);
                    yield return null;
                }

                elapsedTime = 0f;
                duration = .28f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > duration) elapsedTime = duration;
                    FtueHand.position = Vector3.Lerp(posB, posC, elapsedTime / duration);
                    yield return null;
                }

                elapsedTime = 0f;
                duration = 0.98f;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > duration) elapsedTime = duration;
                    FtueHand.position = Vector3.Lerp(posC, posD, elapsedTime / duration);
                    yield return null;
                }

                FtueHandAnimator.SetTrigger(Idle);
                yield return new WaitForSeconds(.6f);
            }
        }

        private void Update()
        {
            if(!FtueActive) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (HoldToBuyCrateFtueActive)
                {
                    DragArea.Instance.TryBuyUnlockableCrateFingerDown();
                }
            }
            
            if (Input.GetMouseButton(0))
            {
                if (DragPlantSeedFtueActive)
                {
                    Ray ray = CameraManager.Instance.farmCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 150f, InputHandler.Instance.farmLayer))
                    {
                        if (hit.transform.TryGetComponent(out FarmArea farmArea))
                        {
                            InputHandler.Instance.OnClickedFarmArea(hit, farmArea, true);
                            if (DragToPlantCount >= 6)
                            {
                                _dragToPlantCompleted = true;
                                DragToPlantSeedFtuePlayed = true;
                                FtueHandAnimator.SetTrigger(Idle);
                                FtueHand.gameObject.SetActive(false);
                            }
                        }
                    }
                }

                if (DragToCollectFtueActive)
                {
                    Ray ray = CameraManager.Instance.farmCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 150f, InputHandler.Instance.farmLayer))
                    {
                        if (hit.transform.TryGetComponent(out FarmArea farmArea))
                        {
                            InputHandler.Instance.OnClickedFarmArea(hit, farmArea, true);
                            if (_collectedPlantCount >= 6)
                            {
                                _dragToCollectCompleted = true;
                                DragToCollectFtuePlayed = true;
                                FtueHandAnimator.SetTrigger(Idle);
                                FtueHand.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (SeedClickedFtueActive)
                {
                    DragArea.Instance.TryClickCrate();
                }
                if (TapToSelectFtueActive)
                {
                    DragAreaSell.Instance.TryClickSellCrate();
                }
                if (TapToShipFtueActive)
                {
                    InputHandler.Instance.TryClickTruck();
                }
                if (CollectMoneyFtueActive)
                {
                    InputHandler.Instance.TryClickTruck();
                }
                if (BuyCargoCarFtueActive)
                {
                    InputHandler.Instance.TryBuyTruck();
                }
                if (HoldToBuyCrateFtueActive)
                {
                    DragArea.Instance.TryBuyUnlockableCrateFingerUp();
                }
            }
        }

        public void AddGrewSeed()
        {
            _grewPlantCount++;
            if (_grewPlantCount >= 6) _waitSeedGrowCompleted = true;
        }

        public void AddCollectedSeed()
        {
            _collectedPlantCount++;
            if (_collectedPlantCount >= 6) _dragToCollectCompleted = true;
        }

        public void MoneyReadyToCollect()
        {
            _waitTruckSellCompleted = true;
        }

        public void MoneyCollected()
        {
            _moneyCollectCompleted = true;
        }

        public void CheckSeedFtueCompleted()
        {
            if (!_seedFtueCompleted)
            {
                _seedFtueCompleted = true;
                SelectSeedFtuePlayed = true;
                FtueHandAnimator.SetTrigger(Idle);
                FtueHand.gameObject.SetActive(false);
            }
        }
        
        public void CheckSelectProductFtueCompleted()
        {
            if (!_selectProductFtueCompleted)
            {
                _selectProductFtueCompleted = true;
                TapToSelectProductFtuePlayed = true;
                FtueHandAnimator.SetTrigger(Idle);
                FtueHand.gameObject.SetActive(false);
            }
        }

        public void CheckTappedShipFtueCompleted()
        {
            if (!_tapToShipFtueCompleted)
            {
                _tapToShipFtueCompleted = true;
                TapToShipProductFtuePlayed = true;
                FtueHandAnimator.SetTrigger(Idle);
                FtueHand.gameObject.SetActive(false);
            }
        }

        public void CheckBuyTruckFtueCompleted()
        {
            if (!_tapToBuyCargoCarCompleted)
            {
                _tapToBuyCargoCarCompleted = true;
                BuyCargoCarFtuePlayed = true;
                FtueHandAnimator.SetTrigger(Idle);
                FtueHand.gameObject.SetActive(false);
            }
        }

        public void CheckHoldToBuyFtueCompleted()
        {
            if (!_holdToBuyCrateCompleted)
            {
                _holdToBuyCrateCompleted = true;
                HoldToBuyCrateFtuePlayed = true;
                FtueHandAnimator3D.SetTrigger(Idle);
                FtueHand3D.gameObject.SetActive(false);
                DragArea.Instance.TryAddCrateSlot();
            }
        }
    }
}