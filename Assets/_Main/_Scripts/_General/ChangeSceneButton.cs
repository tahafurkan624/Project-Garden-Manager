using System.Collections.Generic;
using _Main._Scripts.Managers;
using DG.Tweening;
using HelmetMaster.Main.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts._General
{
    public class ChangeSceneButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private List<Camera> growCameras = new List<Camera>();
        [SerializeField] private List<Camera> sellCameras = new List<Camera>();
        [SerializeField] private TMP_Text changeSceneButtonTMPText;
        [SerializeField] private Image symbolicImage;
        [SerializeField] private Sprite animalSprite, grownSprite, soldSprite;
        [SerializeField] private Transform animationTarget;
        [SerializeField] private List<Transform> crateImages = new List<Transform>();
        private Queue<Transform> activeImages = new Queue<Transform>();

        private MainCanvas _mainCanvas;
        private MainCanvas MainCanvas => _mainCanvas ??= MainCanvas.Instance;
        public bool SellSceneActive { get; private set; }
        
        private bool isAnimalAttacking;
        private int grownFarmSlotCount;
        private int moneyTruckCount;
        private void Start()
        {
            button.onClick.AddListener(OnClick);
            foreach (var crate in crateImages)
            {
                activeImages.Enqueue(crate);
            }
        }

        private void OnClick()
        {
            if (SellSceneActive)
            {
                SellSceneActive = false;
                foreach (var sellCamera in sellCameras)
                { sellCamera.gameObject.SetActive(false); }
                foreach (var growCamera in growCameras)
                { growCamera.gameObject.SetActive(true); }
                changeSceneButtonTMPText.text = "SELL";
                DragArea.Instance.ResetDrag();
                DragArea.IsActive = true;
                DragAreaSell.IsActive = false;
                if (MainCanvas.scrollButton.IsActive)
                {
                    MainCanvas.scrollButton.gameObject.SetActive(true);
                }
            }
            else
            {
                SellSceneActive = true;
                foreach (var sellCamera in sellCameras)
                { sellCamera.gameObject.SetActive(true); }
                foreach (var growCamera in growCameras)
                { growCamera.gameObject.SetActive(false); }
                changeSceneButtonTMPText.text = "GROW";
                DragAreaSell.Instance.ResetDrag();
                DragArea.IsActive = false;
                DragAreaSell.IsActive = true;
                if (MainCanvas.scrollButton.IsActive)
                {
                    MainCanvas.scrollButton.gameObject.SetActive(false);
                }
            }
            UpdateImage();
        }

        public void PlayCrateAnimation(Vector3 crateWorldPos)
        {
            if (activeImages.Count < 1) return;
            
            var image = activeImages.Dequeue();
            
            image.localScale = Vector3.one;
            
            var pos = CameraManager.Instance.GetScreenPosFromDragCam(crateWorldPos);
            image.position = pos;

            image.gameObject.SetActive(true);

            image.DOScale(0.6f, .8f).SetDelay(0.1f);
            image.DOJump(animationTarget.position, 2,1,.8f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                transform.DOComplete();
                transform.DOScale(transform.localScale * 1.1f, 0.2f).SetLoops(2, LoopType.Yoyo);
                activeImages.Enqueue(image);
                image.gameObject.SetActive(false);
            });
            
        }
        
        private void UpdateImage()
        {
            if (SellSceneActive)
            {
                if (isAnimalAttacking)
                {
                    symbolicImage.sprite = animalSprite;
                    if (!symbolicImage.gameObject.activeSelf) symbolicImage.gameObject.SetActive(true);
                }
                else if (grownFarmSlotCount > 0)
                {
                    symbolicImage.sprite = grownSprite;
                    if (!symbolicImage.gameObject.activeSelf) symbolicImage.gameObject.SetActive(true);
                }
                else if (grownFarmSlotCount == 0)
                {
                    symbolicImage.gameObject.SetActive(false);
                }
            }
            else
            {
                if (moneyTruckCount > 0)
                {
                    symbolicImage.sprite = soldSprite;
                    if (!symbolicImage.gameObject.activeSelf) symbolicImage.gameObject.SetActive(true);
                }
                else
                {
                    symbolicImage.gameObject.SetActive(false);
                }
            }
        }
        
        public void AnimalAttacked()
        {
            isAnimalAttacking = true;
            UpdateImage();
        }

        public void AnimalRunoff()
        {
            isAnimalAttacking = false;
            UpdateImage();
        }

        public void SomethingGrown()
        {
            grownFarmSlotCount++;
            UpdateImage();
        }

        public void SomethingGotCollected()
        {
            grownFarmSlotCount--;
            UpdateImage();
        }

        public void SomethingGotStolen()
        {
            grownFarmSlotCount--;
            UpdateImage();
        }
        
        public void SomethingSold()
        {
            moneyTruckCount++;
            UpdateImage();
        }

        public void MoneyCollected()
        {
            moneyTruckCount--;
            UpdateImage();
        }
    }
}