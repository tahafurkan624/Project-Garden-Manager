using System;
using System.Collections;
using System.Collections.Generic;
using _Main._Scripts.Managers;
using _Main._Scripts.Utilities;
using DG.Tweening;
using HelmetMaster.Extensions;
using HelmetMaster.Main;
using HelmetMaster.Main.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace _Main._Scripts._General
{
    public class DragArea : Singleton<DragArea>
    {
        public static bool IsActive;
        public static int ActiveCreateCount;

        [SerializeField] private DragObject unlockableCreateSlotPrefab;
        public List<DragObject> CreateSlotPrefabs = new List<DragObject>();
        public DragObject UnlockableDragObj;

        public CreateSlot SelectedCreateSlot
        {
            get
            {
                if (_selectedCreateSlot == null || !_selectedCreateSlot.HasSeed)
                {
                    for (var index = 0; index < dragObjects.Count - (UnlockableCrateAvailable ? 1 : 0); index++)
                    {
                        var dragObject = dragObjects[index];
                        if (dragObject.CreateSlot.HasSeed)
                        {
                            return dragObject.CreateSlot;
                        }
                    }

                    return UnlockableCrateAvailable ? dragObjects.Count > 1 ? dragObjects[0].CreateSlot :  null : dragObjects.Count > 0 ? dragObjects[0].CreateSlot : null;
                }
                
                
                return _selectedCreateSlot;
            }
        }

        public CreateSlot SelectedCreateSlotAutoSelect
        {
            get
            {
                if (_selectedCreateSlot == null || !_selectedCreateSlot.HasSeed)
                {
                    for (var index = 0; index < dragObjects.Count - (UnlockableCrateAvailable ? 1 : 0); index++)
                    {
                        var dragObject = dragObjects[index];
                        if (dragObject.CreateSlot.HasSeed)
                        {
                            SetDragPositionAndSelect(dragObject.CreateSlot.DragObject);
                            return dragObject.CreateSlot;
                        }
                    }

                    if (UnlockableCrateAvailable)
                    {
                        if (dragObjects.Count > 1)
                        {
                            // set drag parent pos
                            SetDragPositionAndSelect(dragObjects[0].CreateSlot.DragObject);
                            return dragObjects[0].CreateSlot;
                        }

                        return null;
                    }
                    else
                    {
                        if (dragObjects.Count > 0)
                        {
                            // set drag parent pos
                            SetDragPositionAndSelect(dragObjects[0].CreateSlot.DragObject);
                            return dragObjects[0].CreateSlot;
                        }

                        return null;
                    }
                }
                
                
                return _selectedCreateSlot;
            }
        }

        private CreateSlot _selectedCreateSlot;

        [SerializeField] private List<DragObject> dragObjects = new List<DragObject>();
        [SerializeField] private Transform dragParent;

        private bool _isDragging, _isTweening, _isBuyingCreate;
        public bool Dragging => _isTweening || _isDragging;
        public bool BigSeedsEnabled;

        [SerializeField] public LayerMask dragLayer;

        [SerializeField] private Camera cam;

        public CreateSlotUnlockable SelectedSlotUnlockable;
        private Vector3 _FingerDownPos, _dragEndParentPos, _parentStartPos;//, _selectedObjectOffset = Vector3.zero;
        private float _rayDis = 150f;

        protected override void Awake()
        {
            base.Awake();
            BigSeedsEnabled = GlobalPlayerPrefs.HasBoughtBigTruckBefore;
            //foreach (var dragObject in dragObjects) { dragObject.Init(); }
            IsActive = true;
            _parentStartPos = dragParent.position;
            _dragEndParentPos = dragParent.position;
        }

        public void GenerateStartCrates()
        {
            ActiveCreateCount = 0;
            for (int i = 0; i < GlobalPlayerPrefs.SavedItems.TotalCreateAmount-2; i++)
            {
                AddCrateSlot();
            }

            InstantAdjust();
            CheckUnlockableCrate();
        }

        public void OnFruitsReady()
        {
            this.Invoke(() =>
            {
                if (ActiveCreateCount < GlobalPlayerPrefs.SavedItems.TotalCreateAmount)
                {
                    // make it a Coroutine
                    TryAddCrateSlot();
                }
            }, 3f);
        }

        public void OnCrateSend(DragObject dragObject)  // On Crate Send
        {
            OnFruitsReady();
            //if (_selectedCreateSlot.DragObject == dragObject) _selectedCreateSlot = null;
            MainCanvas.Instance.changeSceneButton.PlayCrateAnimation(dragObject.transform.position);
            DragAreaSell.Instance.TransferCreateSlot(dragObject.CreateSlot);
            dragObjects.Remove(dragObject);
            AdjustCreatePositions();
            //AddCreateSlot();
        }

        public void OnCrateShipped(bool ftueActive = false)
        {
            //if (_selectedCreateSlot.DragObject == dragObject) _selectedCreateSlot = null;
            ActiveCreateCount--; // !! ??

            if (ftueActive) ForceAddCrateSlot();
            else TryAddCrateSlot();
        }

        public void AdjustCreatePositions()
        {
            if (UnlockableDragObj != null)
            {
                dragObjects.Remove(UnlockableDragObj);
                dragObjects.Add(UnlockableDragObj);
            }
            
            List<Vector3> dragPositions = new List<Vector3>();
            for (var i = 0; i < dragObjects.Count; i++)
            {
                if (i == 0)
                {
                    dragPositions.Add(dragParent.position - Vector3.right);
                    continue;
                }
                
                var dragObject = dragObjects[i];
                var preDragObject = dragObjects[i - 1];

                Vector3 offset = (Vector3.right * preDragObject.objectScale) / 2 
                                 + (Vector3.right * dragObject.objectScale) / 2;

                dragPositions.Add(dragPositions[^1] + offset);
            }
            for (var i = 0; i < dragObjects.Count; i++)
            {
                var dragObject = dragObjects[i];
                //dragObject.transform.position = dragParent.position +  Vector3.right * (i * objectScale);
                dragObject.transform.DOComplete();
                dragObject.transform.DOMove(dragPositions[i] , .32f);
                // Maybe Local ??
                //dragObject.Init(dragObject.transform.position.x);
            }
        }

        private void InstantAdjust()
        {
            if (UnlockableDragObj != null)
            {
                dragObjects.Remove(UnlockableDragObj);
                dragObjects.Add(UnlockableDragObj);
            }
            
            for (var i = 0; i < dragObjects.Count; i++)
            {
                var dragObject = dragObjects[i];

                if (i == 0)
                {
                    dragObject.transform.position = dragParent.position - Vector3.right;
                    continue;
                }
                
                var preDragObject = dragObjects[i - 1];

                Vector3 offset = (Vector3.right * preDragObject.objectScale) / 2 
                                 + (Vector3.right * dragObject.objectScale) / 2;

                dragObject.transform.position = preDragObject.transform.position + offset;
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddUnlockableCrateSlot();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                AddCrateSlot();
            }

            if(!IsActive) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                TryBuyCrate();

                TrySelectCreateSlot();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isBuyingCreate = false;
                // Snap
                if (!_isDragging) return;

                _isTweening = true;
                _isDragging = false;
                _dragEndParentPos = dragParent.position;

                //TrySelectCreateSlot();
            }

            if (_isDragging)
            {
                RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
                {
                    var pos = dragParent.position;
                    var delta = hit.point - _FingerDownPos;
                    //delta *= 2; // multiplier
                    delta += _dragEndParentPos;
                    Vector3 endPos;
                    var averageScale = 0f;
                    foreach (var drgObj in dragObjects)
                    {
                        averageScale += drgObj.objectScale;
                    }

                    averageScale /= dragObjects.Count;
                    
                    endPos = new Vector3(Mathf.Clamp(delta.x, -dragObjects.Count * averageScale + averageScale/2 + 1, 0), pos.y,pos.z);;
                    dragParent.position = endPos;
                }
            }
        }

        private IEnumerator BuyCoroutine(CreateSlotUnlockable crateSlotUnlockable, bool ftueActive = false)
        {
            _isBuyingCreate = true;
            var fillAmount = 0f;
            while (GameEconomy.Instance.HasEnoughMoney(crateSlotUnlockable.Price) && _isBuyingCreate && fillAmount < 1)
            {
                fillAmount+= Time.deltaTime;
                crateSlotUnlockable.Mat.SetFloat("_Grow", fillAmount);
                //createSlot.FillImage.fillAmount = fillAmount;
                yield return null;
            }

            crateSlotUnlockable.Mat.SetFloat("_Grow", 0);
            if (fillAmount < 1) yield break;

            if (GameEconomy.Instance.HasEnoughMoney(crateSlotUnlockable.Price))
            {
                GameEconomy.Instance.SpendMoney(crateSlotUnlockable.Price);
                crateSlotUnlockable.Unlock();
                if (ftueActive) FtueManager.Instance.CheckHoldToBuyFtueCompleted();
            }
        }

        private void SetSlot(CreateSlotUnlockable createSlotUnlockable)
        {
            SelectedSlotUnlockable = createSlotUnlockable;
        }

        public void RemoveDragObject(DragObject dragObject)
        {
            dragObjects.Remove(dragObject);
        }

        public void TryAddCrateSlot()
        {
            if (FtueManager.FtueActive) return;

            if (ActiveCreateCount < GlobalPlayerPrefs.SavedItems.TotalCreateAmount)
            {
                AddCrateSlot();
                AdjustCreatePositions();
            }
        }

        public void ForceAddCrateSlot()
        {
            AddCrateSlot();
            AdjustCreatePositions();
        }

        public void AddCrateSlot()
        {
            var crate = GetRandomCrate();
            Vector3 pos = dragParent.position;
            if (dragObjects.Count > 1)
            {
                pos = dragObjects[^1].transform.position + Vector3.right * crate.objectScale;
            }
            var dragObj = Instantiate(crate, pos, Quaternion.identity, dragParent);
            //dragObj.Init(pos.x);
            // dragObj.CreateSlot.DragArea = this;
            dragObjects.Add(dragObj);
            ActiveCreateCount++;
        }

        private DragObject GetRandomCrate()
        {
            List<DragObject> doubleCrates = new List<DragObject>();
            List<DragObject> singleCrates = new List<DragObject>();

            foreach (var crate in CreateSlotPrefabs)
            {
                if (Math.Abs(crate.CreateSlot.seedBags[0].Seed.size.x - 1) > 0.05f)
                {
                    doubleCrates.Add(crate);
                }
                else
                {
                    singleCrates.Add(crate);
                }
            }

            if (BigSeedsEnabled)
            {
                var random = Random.Range(0, 10);
                if (random > 6 && doubleCrates.Count > 0)
                {
                    return doubleCrates.RandomItem();
                }
            
                return singleCrates.RandomItem();
            }
            else
            {
                return singleCrates.RandomItem();
            }
        }
        
        private void AddUnlockableCrateSlot()
        {
            UnlockableCrateAvailable = true;
            
            Vector3 offset = Vector3.zero;
            for (int j = 0; j < dragObjects.Count; j++)
            {
                offset += Vector3.right * dragObjects[j].objectScale;
            }
            
            var dragObj = Instantiate(unlockableCreateSlotPrefab,
                dragParent.position + offset, quaternion.identity, dragParent);
            //dragObj.Init(dragObj.transform.position.x);
            UnlockableDragObj = dragObj;
            dragObjects.Add(dragObj);
        }

        public void AddNewUnlockableCrateSlot()
        {
            GlobalPlayerPrefs.SavedItems.UnlockableCrateAmount+=3;
            CheckUnlockableCrate();
        }

        public bool UnlockableCrateAvailable;

        public void CheckUnlockableCrate()
        {
            if (GlobalPlayerPrefs.SavedItems.UnlockableCrateAmount > GlobalPlayerPrefs.SavedItems.TotalCreateAmount && !UnlockableCrateAvailable)
            {
                AddUnlockableCrateSlot();
            }
        }

        public void ResetDrag()
        {
            dragParent.position = _parentStartPos;
            _dragEndParentPos = _parentStartPos;
        }

        private void TrySelectCreateSlot()
        {
            RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
            {
                if (hit.transform.TryGetComponent(out CreateSlot slot))
                {
                    if (_selectedCreateSlot != null)
                    {
                        _selectedCreateSlot.transform.GetChild(0).DOComplete();
                        _selectedCreateSlot.transform.GetChild(0).DOLocalMoveY(0, .5f);
                    }
                    _selectedCreateSlot = slot;
                    slot.transform.GetChild(0).DOComplete();
                    slot.transform.GetChild(0).DOLocalMoveY(.2f, .5f).SetEase(Ease.OutBack);
                }
            }
        }

        private void TryBuyCrate()
        {
            RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
            {
                if (hit.transform.TryGetComponent(out CreateSlotUnlockable createSlot) && !createSlot.HasBought)
                {
                    if (GameEconomy.Instance.HasEnoughMoney(createSlot.Price)) StartCoroutine(BuyCoroutine(createSlot));
                }
                else
                {
                    dragParent.DOComplete();
                    //DOTween.Complete("DragSelectPos");
                    _FingerDownPos = hit.point;
                    _isDragging = true;
                }
            }
        }

        public void TryClickCrate()
        {
            RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
            {
                if (hit.transform.TryGetComponent(out CreateSlot slot))
                {
                    if (_selectedCreateSlot != null)
                    {
                        _selectedCreateSlot.transform.GetChild(0).DOComplete();
                        _selectedCreateSlot.transform.GetChild(0).DOLocalMoveY(0, .5f);
                        FtueManager.Instance.CheckSeedFtueCompleted();
                    }
                    _selectedCreateSlot = slot;
                    slot.transform.GetChild(0).DOComplete();
                    slot.transform.GetChild(0).DOLocalMoveY(.2f, .5f).SetEase(Ease.OutBack);
                    FtueManager.Instance.CheckSeedFtueCompleted();
                }
            }
        }

        public void TryBuyUnlockableCrateFingerUp()
        {
            _isBuyingCreate = false;
        }
        public void TryBuyUnlockableCrateFingerDown()
        {
            RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
            {
                if (hit.transform.TryGetComponent(out CreateSlotUnlockable createSlot) && !createSlot.HasBought)
                {
                    if (GameEconomy.Instance.HasEnoughMoney(createSlot.Price)) StartCoroutine(BuyCoroutine(createSlot, true));
                }
                else
                {
                    dragParent.DOComplete();
                    _FingerDownPos = hit.point;
                    _isDragging = true;
                }
            }
        }

        public void AdjustDragParentPosForFtue()
        {
            dragParent.position += Vector3.right * - 2.6f;
            _dragEndParentPos = dragParent.position;
        }

        private DragObject _focusedDragObj;
        public void AdjustDragParentPos(DragObject dragObject)
        {
            if (_focusedDragObj == null)
            {
                _focusedDragObj = dragObject;
                var delta = dragParent.position -  dragObject.transform.position;
                var dragParentPos = dragParent.position;
                delta.y = dragParentPos.y;
                delta.z = dragParentPos.z;
                dragParent.DOKill();
                dragParent.DOMove(delta, .8f).SetEase(Ease.OutQuad)
                    .OnKill(()=>_dragEndParentPos = dragParent.position).OnComplete(()=>_dragEndParentPos = dragParent.position);
            }
            else if(_focusedDragObj != dragObject)
            {
                _focusedDragObj = dragObject;
                var delta = dragParent.position -  dragObject.transform.position;
                var dragParentPos = dragParent.position;
                delta.y = dragParentPos.y;
                delta.z = dragParentPos.z;
                dragParent.DOKill();
                dragParent.DOMove(delta, .8f).SetEase(Ease.OutQuad)
                    .OnKill(()=>_dragEndParentPos = dragParent.position).OnComplete(()=>_dragEndParentPos = dragParent.position);
            }
        }

        private void SetDragPositionAndSelect(DragObject dragObject)
        {
            if (_selectedCreateSlot != dragObject.CreateSlot)
            {
                _selectedCreateSlot = dragObject.CreateSlot;
                _selectedCreateSlot.transform.GetChild(0).DOComplete();
                _selectedCreateSlot.transform.GetChild(0).DOLocalMoveY(0, .5f);

                //var delta = dragObject.transform.position - dragParent.position;
                var delta = dragParent.position -  dragObject.transform.position;
                var dragParentPos = dragParent.position;
                delta.y = dragParentPos.y;
                delta.z = dragParentPos.z;
                //dragParent.position = delta;
                //DOTween.Complete("DragSelectPos");
                // if (CameraManager.Instance.IsInsideDragCam(delta))
                // {
                //     
                // }
                // else
                // {
                //     
                // }
                dragParent.DOComplete();
                dragParent.DOMove(delta, .8f).SetEase(Ease.OutQuad)/*.SetId("DragSelectPos")*/
                    .OnComplete(()=>_dragEndParentPos = dragParent.position);
            }
        }
    }
}