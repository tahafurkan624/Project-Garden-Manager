using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.Managers;
using _Main._Scripts.Utilities;
using DG.Tweening;
using HelmetMaster.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts._General
{
    public class LevelProgress : Singleton<LevelProgress>
    {
        public int XP
        {
            get => PlayerPrefs.GetInt("TotalXP", 0);
            set => PlayerPrefs.SetInt("TotalXP", value);
        }

        public List<int> LevelXPTargets = new List<int>();

        [SerializeField] private TMP_Text LevelIdxTMPText;
        [SerializeField] private Image fillImage, levelImage, levelImageOnBar;
        [SerializeField] private List<Sprite> levelSprites = new List<Sprite>();
        [SerializeField] private GameObject levelEndPanel;
        [SerializeField] private Transform TwoCarCamPos, FourCarCamPos, NineCarCamPos;
        [SerializeField] private List<TruckArea> trucks = new List<TruckArea>();

        [SerializeField] private DragObject WaterMelonPrefab;
        [SerializeField] private DragObject CarrotPrefab;
        [SerializeField] private DragObject PumpkinPrefab;
        [SerializeField] private DragObject EggplantPrefab;
        [SerializeField] private DragObject CabbagePrefab;
        [SerializeField] private DragObject PepperPrefab;

        public int ReachedLvlXP => LevelXPTargets[Mathf.Clamp(ReachedLevelIdx, 0, LevelXPTargets.Max())];

        public int ReachedLevelIdx
        {
            get => PlayerPrefs.GetInt("ReachedLevelIdx", 1);
            set => PlayerPrefs.SetInt("ReachedLevelIdx", value);
        }

        protected override void Awake()
        {
            base.Awake();

            LevelIdxTMPText.text = (ReachedLevelIdx).ToString();
            fillImage.fillAmount = Helper.Remap(LevelXPTargets[ReachedLevelIdx - 1], ReachedLvlXP, 0, 1, XP);
            levelImageOnBar.sprite = levelSprites[ReachedLevelIdx-1];
        }

        private void Start()
        {
            if (ReachedLevelIdx >= 2)
            {
                CameraManager.Instance.sellCam.transform.position = FourCarCamPos.position;
                trucks[2].gameObject.SetActive(true);
                trucks[3].gameObject.SetActive(true);
            }
            if (ReachedLevelIdx >= 3)
            {
                DragArea.Instance.CreateSlotPrefabs.Add(WaterMelonPrefab);
            }
            if (ReachedLevelIdx >= 5)
            {
                DragArea.Instance.CreateSlotPrefabs.Add(CarrotPrefab);
            }
            if (ReachedLevelIdx >= 6)
            {
                CameraManager.Instance.sellCam.transform.position = NineCarCamPos.position;
                trucks[4].gameObject.SetActive(true);
                trucks[5].gameObject.SetActive(true);
                trucks[6].gameObject.SetActive(true);
                trucks[7].gameObject.SetActive(true);
                trucks[8].gameObject.SetActive(true);
            }
            if (ReachedLevelIdx >= 8)
            {
                DragArea.Instance.CreateSlotPrefabs.Add(PumpkinPrefab);
            }
            if (ReachedLevelIdx >= 10)
            {
                DragArea.Instance.CreateSlotPrefabs.Add(EggplantPrefab);
            }
            if (ReachedLevelIdx >= 11)
            {
                DragArea.Instance.CreateSlotPrefabs.Add(CabbagePrefab);
            }
            if (ReachedLevelIdx >= 13)
            {
                DragArea.Instance.CreateSlotPrefabs.Add(PepperPrefab);
            }

            DragArea.Instance.GenerateStartCrates();
        }

        public void EarnXp(int amount)
        {
            XP += amount;
            // while (XP > ReachedLvlXP)
            // {
            //     ReachedLevelIdx++;
            // }

            fillImage.fillAmount = Helper.Remap(LevelXPTargets[ReachedLevelIdx - 1], ReachedLvlXP, 0, 1, XP);

            if (XP < ReachedLvlXP) return;

            ReachedLevelIdx++;
            LevelIdxTMPText.text = (ReachedLevelIdx).ToString();
            SetupLevelEndPanel();
            fillImage.fillAmount = Helper.Remap(LevelXPTargets[ReachedLevelIdx - 1], ReachedLvlXP, 0, 1, XP);
                //Mathf.Lerp(XP, LevelXPTargets[ReachedLevelIdx-1], ReachedLvlXP);
        }

        private void SetupLevelEndPanel()
        {
            switch (ReachedLevelIdx)
            {
                case 0:     // Nope
                    break;
                case 2:     // More Cars
                    CameraManager.Instance.sellCam.transform.position = FourCarCamPos.position;
                    trucks[2].gameObject.SetActive(true);
                    trucks[3].gameObject.SetActive(true);
                    break;
                case 3:     // WaterMelon
                    DragArea.Instance.CreateSlotPrefabs.Add(WaterMelonPrefab);
                    break;
                case 4:     // Crate
                    AnimalManager.Instance.OnReachedLevel4();
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 5:     // Carrot
                    DragArea.Instance.CreateSlotPrefabs.Add(CarrotPrefab);
                    break;
                case 6:     // Car
                    CameraManager.Instance.sellCam.transform.position = NineCarCamPos.position;
                    trucks[4].gameObject.SetActive(true);
                    trucks[5].gameObject.SetActive(true);
                    trucks[6].gameObject.SetActive(true);
                    trucks[7].gameObject.SetActive(true);
                    trucks[8].gameObject.SetActive(true);
                    break;
                case 7:     // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 8:     // Pumpkin
                    DragArea.Instance.CreateSlotPrefabs.Add(PumpkinPrefab);
                    break;
                case 9:     // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 10:     // Eggplant
                    DragArea.Instance.CreateSlotPrefabs.Add(EggplantPrefab);
                    break;
                case 11:    // Cabbage
                    DragArea.Instance.CreateSlotPrefabs.Add(CabbagePrefab);
                    break;
                case 12:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 13:    // Pepper
                    DragArea.Instance.CreateSlotPrefabs.Add(PepperPrefab);
                    break;
                case 14:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 15:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 16:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 17:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 18:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 19:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 20:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                case 21:    // Crate
                    DragArea.Instance.AddNewUnlockableCrateSlot();
                    break;
                default:
                    break;
            }
            levelImage.sprite = levelSprites[ReachedLevelIdx - 2];
            levelImageOnBar.sprite = levelSprites[ReachedLevelIdx - 1];
            var scale = levelEndPanel.transform.localScale;
            levelEndPanel.transform.localScale = Vector3.zero;
            levelEndPanel.transform.DOScale(scale, .8f).SetEase(Ease.OutBack);
            levelEndPanel.SetActive(true);
        }
    }
}