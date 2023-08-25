using System.Collections.Generic;
using _Main._Scripts.Managers;
using _Main._Scripts.Utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


namespace _Main._Scripts._General
{
    public class DragAreaSell : Singleton<DragAreaSell>
    {
        public static bool IsActive;
        public static int ActiveCreateCount;

        private CreateSlot _selectedCreateSlot;

        public CreateSlot SelectedCreateSlot
        {
            get
            {
                if (_selectedCreateSlot == null)
                {
                    foreach (var dragObject in dragObjects)
                    {
                        if (dragObject.CreateSlot.HasSeed)
                        {
                            return dragObject.CreateSlot;
                        }
                    }

                    if (dragObjects.Count < 1) return null;
                    return dragObjects[0].CreateSlot;
                }
                
                return _selectedCreateSlot;
            }
        }
        
        [SerializeField] private List<DragObject> dragObjects = new List<DragObject>();
        [SerializeField] private Transform dragParent;

        private bool _isDragging;

        [SerializeField] private LayerMask dragLayer;
        //[SerializeField] private float objectScale;

        [SerializeField] private Camera cam;

        private Vector3 _FingerDownPos, _dragEndParentPos, _parentStartPos;
        private float _rayDis = 150f;

        protected override void Awake()
        {
            base.Awake();
            _parentStartPos = dragParent.position;
            //foreach (var dragObject in dragObjects) { dragObject.Init(); }
            IsActive = false;
        }

        // public void AdjustCreatePositions()
        // {
        //     for (var i = 0; i < dragObjects.Count; i++)
        //     {
        //         var dragObject = dragObjects[i];
        //         dragObject.transform.position = dragParent.position +  Vector3.right * (i * objectScale);
        //         dragObject.Init(i * objectScale);
        //     }
        // }

        public void AdjustCreatePositions()
        {
            List<Vector3> dragPositions = new List<Vector3>();
            for (var i = 0; i < dragObjects.Count; i++)
            {
                if (i == 0)
                {
                    dragPositions.Add(dragParent.position);
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
        
        private void Update()
        {
            if(!IsActive) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
                {
                    dragParent.DOComplete();
                    _FingerDownPos = hit.point;
                    _isDragging = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!_isDragging) return;

                _isDragging = false;
                _dragEndParentPos = dragParent.position;

                RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
                {
                    if (hit.transform.TryGetComponent(out CreateSlot slot))
                    {
                        if (_selectedCreateSlot != null)
                        {
                            _selectedCreateSlot.transform.DOComplete();
                            _selectedCreateSlot.transform.DOLocalMoveY(0, .5f);
                        }
                        _selectedCreateSlot = slot;
                        slot.transform.DOComplete();
                        slot.transform.DOLocalMoveY(.2f, .5f).SetEase(Ease.OutBack);
                    }
                }
            }

            if (_isDragging)
            {
                RaycastHit hit; Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
                {
                    var pos = dragParent.position;
                    var delta = hit.point - _FingerDownPos;
                    //delta *= 2;
                    delta += _dragEndParentPos;
                    Vector3 endPos;
                    if (dragObjects.Count < 2) endPos = pos;
                    else
                    {
                        var averageScale = 0f;
                        foreach (var drgObj in dragObjects)
                        {
                            averageScale += drgObj.objectScale;
                        }

                        averageScale /= dragObjects.Count;
                        endPos = new Vector3(Mathf.Clamp(delta.x, -dragObjects.Count * averageScale + averageScale/2 + 1, 0), pos.y,pos.z);
                    }
                    dragParent.position = endPos;
                }
            }
        }

        public void RemoveDragObject(DragObject dragObject)
        {
            dragObjects.Remove(dragObject);
        }

        public void TransferCreateSlot(CreateSlot slot)
        {
            var dragObj = slot.DragObject;
            dragObj.transform.DOKill();
            dragObj.transform.SetParent(dragParent);
            Vector3 pos = dragParent.position;
            if (dragObjects.Count > 1)
            {
                pos = dragObjects[^1].transform.position + Vector3.right * dragObj.objectScale;
            }
            
            dragObj.transform.position = pos;
            //dragObj.Init(dragObjects.Count * objectScale);
            //dragObj.CreateSlot.DragArea = this;
            dragObjects.Add(dragObj);
            ActiveCreateCount++;
            AdjustCreatePositions();
        }

        public void SetSelectedCreateSlotNull()
        {
            _selectedCreateSlot = null;
        }

        public void ResetDrag()
        {
            dragParent.position = _parentStartPos;
            _dragEndParentPos = _parentStartPos;
        }

        public void TryClickSellCrate()
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, _rayDis, dragLayer))
            {
                if (hit.transform.TryGetComponent(out CreateSlot slot))
                {
                    if (_selectedCreateSlot != null)
                    {
                        _selectedCreateSlot.transform.DOComplete();
                        _selectedCreateSlot.transform.DOLocalMoveY(0, .5f);
                        FtueManager.Instance.CheckSelectProductFtueCompleted();
                    }
                    _selectedCreateSlot = slot;
                    slot.transform.DOComplete();
                    slot.transform.DOLocalMoveY(.2f, .5f).SetEase(Ease.OutBack);
                    FtueManager.Instance.CheckSelectProductFtueCompleted();
                }
            }
        }
    }
}