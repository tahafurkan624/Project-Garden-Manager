using _Main._Scripts._General.FarmingSystem;
using _Main._Scripts._General.AnimalSystem;
using _Main._Scripts.Managers;
using _Main._Scripts.Utilities;
using HelmetMaster.Main;
using HelmetMaster.Main.UI;
using UnityEngine;

namespace _Main._Scripts._General
{
    public class InputHandler : Singleton<InputHandler>
    {
        public LayerMask farmLayer, unlockableFarmLayer, animalLayer;

        private CreateSlot _selectedCrateSlot;

        private Camera MainCamera => MainCanvas.Instance.changeSceneButton.SellSceneActive ? sellCam : growCam;
        [SerializeField] private Camera growCam, sellCam;

        private bool isLastInputWasCollect, isLastInputWasAnimal, isLastInputCollectMoney;
        
        public static bool IsActive;
        private int frameCount = 0;
        private bool isMeantToClickUnlockableFarmArea;
        
        protected override void Awake()
        {
            base.Awake();
            IsActive = true;
        }

        private void Update()
        {
            if (!IsActive) return;

            if (Input.GetKeyDown(KeyCode.O))
            {
                Time.timeScale = 5;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = 1;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 150f, animalLayer))
                {
                    if (hit.transform.TryGetComponent(out Animal animal))
                    {
                        isLastInputWasAnimal = true;
                        animal.OnClick();
                    }
                }
                
                if (Physics.Raycast(ray, out RaycastHit hit2, 150f, unlockableFarmLayer))
                {
                    if (hit2.transform.TryGetComponent(out FarmAreaUnlockable row))
                    {
                        isMeantToClickUnlockableFarmArea = true;
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 150f, farmLayer))
                {
                    if (hit.transform.TryGetComponent(out FarmArea farmArea))
                    {
                        OnClickedFarmArea(hit, farmArea);
                    }
                    else if (hit.transform.parent.TryGetComponent(out TruckArea truckArea))
                    {
                       OnClickedTruckArea(hit, truckArea);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _selectedCrateSlot = null;
                isLastInputWasCollect = false;
                isLastInputWasAnimal = false;
                isLastInputCollectMoney = false;
                
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                if (isMeantToClickUnlockableFarmArea)
                {
                    if (Physics.Raycast(ray, out RaycastHit hit, 150f, unlockableFarmLayer))
                    {
                        if (hit.transform.TryGetComponent(out FarmAreaUnlockable row))
                        {
                            row.OnClick();
                        }
                    }
                    isMeantToClickUnlockableFarmArea = false;
                }
                
                
                if (Physics.Raycast(ray, out RaycastHit hit2, 150f, farmLayer))
                {
                    if (hit2.transform.parent.TryGetComponent(out TruckArea truckArea))
                    {
                        if (!truckArea.IsUnlocked)
                        {
                            if (!GameEconomy.Instance.HasEnoughMoney(truckArea.Price)) return;
                            GameEconomy.Instance.SpendMoney(truckArea.Price);
                            truckArea.Unlock();
                        }
                    }
                }
            }
        }

        public void OnClickedFarmArea(RaycastHit hit, FarmArea farmArea, bool FtueActive = false)
        {
            var slot = farmArea.GetNearestGridSlot(hit.point);

            if (slot.IsLocked) return;
            
            var currentCrate = DragArea.Instance.SelectedCreateSlot;

            if(currentCrate == null) return;

            if (_selectedCrateSlot != null && (_selectedCrateSlot == null || currentCrate != _selectedCrateSlot)) return;

            if (slot.IsOccupied)
            {
                if (slot.currentSeed.IsGrown)
                {
                    isLastInputWasCollect = true;
                    MainCanvas.Instance.changeSceneButton.SomethingGotCollected();
                    slot.currentSeed.Collect();
                    if (FtueActive) FtueManager.Instance.AddCollectedSeed();
                }
            }
            
            if (isLastInputWasCollect || isLastInputWasAnimal)
            {
                // it was collecting on last input
            }
            else if (currentCrate.HasSeed && slot.CanPlaceObject(currentCrate.NextSeedBag()))
            {
                _selectedCrateSlot = DragArea.Instance.SelectedCreateSlotAutoSelect;
                _selectedCrateSlot = currentCrate;
                var seedBag = currentCrate.GetNextSeedBag();
                seedBag.Plant(slot.SpawnPos, FtueActive);
                AnimalManager.Instance.OnSomethingPlanted(farmArea, slot);
                if (FtueActive) FtueManager.Instance.DragToPlantCount++;
            }
        }

        private void OnClickedTruckArea(RaycastHit hit, TruckArea truckArea, bool FtueActive = false)
        {
            if (!truckArea.IsUnlocked) return;

            if(truckArea.IsLoaded) return;

            if (truckArea.ReadyToSell)
            {
                isLastInputCollectMoney = true;
                truckArea.CollectMoney(FtueActive);
            }
            else if (isLastInputCollectMoney)
            {
                // last input was collecting money
            }
            else
            {
                var crate = DragAreaSell.Instance.SelectedCreateSlot;
                if(crate == null) return;

                if (!truckArea.isBigTruck && crate.type is CreateSlot.ProductTypes.Cabbage or 
                    CreateSlot.ProductTypes.Pumpkin or CreateSlot.ProductTypes.Watermelon) 
                {
                    if (!crate.IsHapticPlaying)
                    {
                        truckArea.CantPlaceThatCrate();
                        crate.CantPlaceThere();
                    }
                   
                    return;
                }
                truckArea.LoadCrateToTruck(crate, FtueActive);
                DragAreaSell.Instance.SetSelectedCreateSlotNull();
                DragAreaSell.Instance.RemoveDragObject(crate.DragObject);
                DragAreaSell.Instance.AdjustCreatePositions();
            }
        }

        public void TryClickTruck()
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 150f, farmLayer))
            {
                if (hit.transform.parent.TryGetComponent(out TruckArea truckArea))
                {
                    OnClickedTruckArea(hit, truckArea, true);
                }
            }
        }

        public void TryBuyTruck()
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit2, 150f, farmLayer))
            {
                if (hit2.transform.parent.TryGetComponent(out TruckArea truckArea))
                {
                    if (!truckArea.IsUnlocked)
                    {
                        if (!GameEconomy.Instance.HasEnoughMoney(truckArea.Price)) return;
                        GameEconomy.Instance.SpendMoney(truckArea.Price);
                        truckArea.Unlock();
                        FtueManager.Instance.CheckBuyTruckFtueCompleted();
                    }
                }
            }
        }
    }
}