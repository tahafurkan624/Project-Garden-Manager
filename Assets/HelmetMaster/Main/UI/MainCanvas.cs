using System.Collections.Generic;
using _Main._Scripts._General;
using _Main._Scripts.Managers;
using _Main._Scripts.Utilities;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HelmetMaster.Main.UI
{
    public class MainCanvas : Singleton<MainCanvas>
    {
        public bool TapToStartActivity;

        [SerializeField] private GameObject successPanel;
        [SerializeField] private GameObject failPanel;
        [SerializeField] private GameObject tapToStartPanel;
        [SerializeField] private Button successButton;
        [SerializeField] private Button failButton;
        [SerializeField] private Button tapToStartButton;
        [SerializeField] private TMP_Text moneyTMPText;
        
        public ScrollButton scrollButton;
        public ChangeSceneButton changeSceneButton;

        protected override void Awake()
        {
            base.Awake();
            successButton.onClick.AddListener(() =>
            {
                successButton.interactable = false;
                if(!GameManager.Instance.TryLoadTutorialScene()) GameManager.Instance.LoadReachedLevel();
            });
            failButton.onClick.AddListener(() =>
            {
                failButton.interactable = false;
                GameManager.Instance.LoadCurrentLevel();
            });
            tapToStartButton.onClick.AddListener(() =>
            {
                tapToStartButton.interactable = false;
                GameManager.Instance.ChangeState(GameState.Playing);
                tapToStartPanel.SetActive(false);
            });
            
        }

        public void EnableSuccessUI()
        {
            successPanel.SetActive(true);
        }

        public void EnableFailUI()
        {
            failPanel.SetActive(true);
        }

        public void EnableTapToStartUI()
        {
            tapToStartPanel.SetActive(true);
        }

        public void SetMoneyTextAnimated(int from, int to)
        {
            DOTween.Kill("MoneyTextAnim");
            DOTween.To(() => from, x => from = x, to, .4f).SetId("MoneyTextAnim")
                .SetEase(Ease.Linear).OnUpdate(() => { moneyTMPText.text = from.FormatMoney(); });
        }

        public void SetMoneyText(int nextMoney)
        {
            moneyTMPText.text = nextMoney.FormatMoney();
        }
    }
}