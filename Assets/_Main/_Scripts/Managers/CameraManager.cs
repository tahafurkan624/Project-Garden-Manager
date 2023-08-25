using _Main._Scripts.Utilities;
using DG.Tweening;
using UnityEngine;

namespace _Main._Scripts.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] public Camera farmCam;
        [SerializeField] public Camera dragCam;
        [SerializeField] public Camera sellCam;
        [SerializeField] public Camera dragSellCam;
        [SerializeField] private Transform firstFarmCamTarget, secondFarmCamTarget;
        
        public Vector3 GetDragCamPos(Vector3 pos)
        {
            var farmCamViewPos = farmCam.WorldToScreenPoint(pos);
            return dragCam.ScreenToWorldPoint(farmCamViewPos);
        }

        public Vector3 GetFarmCamPos(Vector3 pos)
        {
            var dragCamViewPos = dragCam.WorldToScreenPoint(pos);
            return farmCam.ScreenToWorldPoint(dragCamViewPos);
        }
        
        public Vector3 GetDragSellCamPos(Vector3 pos)
        {
            var dragCamViewPos = sellCam.WorldToScreenPoint(pos);
            return dragSellCam.ScreenToWorldPoint(dragCamViewPos);
        }
        
        public Vector3 GetSellCamPos(Vector3 pos)
        {
            var dragCamViewPos = dragSellCam.WorldToScreenPoint(pos);
            return sellCam.ScreenToWorldPoint(dragCamViewPos);
        }

        public Vector3 GetScreenPosFromFarmCam(Vector3 pos)
        {
            return farmCam.WorldToScreenPoint(pos);
        }

        public bool IsInsideDragCam(Vector3 pos)
        {
            var value = dragCam.WorldToViewportPoint(pos);
            if (value.x < 0 || value.x > 1 || value.y < 0 || value.y > 1)
            {
                return false;
            }

            return true;
        }
        
        public Vector3 GetScreenPosFromDragCam(Vector3 pos)
        {
            return dragCam.WorldToScreenPoint(pos);
        }
        
        public void ScrollFarmCamUp()
        {
            farmCam.transform.DOKill();
            farmCam.transform.DOMove(secondFarmCamTarget.position, 1f);
        }

        public void ScrollFarmCamDown()
        {
            farmCam.transform.DOKill();
            farmCam.transform.DOMove(firstFarmCamTarget.position, 1f);
        }
    }
}