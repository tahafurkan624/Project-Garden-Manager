using DG.Tweening;
using HelmetMaster.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace zz_HelmetMaster.IncrementalButtons
{
    public class IncrementalButton : MonoBehaviour
    {
        [Tooltip("If script is on the same GameObject with Button component, you can leave this empty")] [SerializeField] private Button _button;

        [Tooltip("Give different ID's for every button")] [SerializeField] private string buttonID;

        [Tooltip("ButtonPriceList SO")] [SerializeField] private ButtonPriceList priceList;
        
        [Tooltip("Overlay to show that button is not interactable")] [SerializeField] private GameObject overlay;
        
        
        private Button Button => _button ??= GetComponent<Button>();

        public UnityEvent onBought;
        private int CurrentLevel
        {
            get => PlayerPrefs.GetInt($"lvl_{buttonID}", 0);
            set => PlayerPrefs.SetInt($"lvl_{buttonID}", value);
        }
        
        private int CurrentPrice => (int)priceList.prices[(CurrentLevel >= priceList.prices.Count)? priceList.prices.Count - 1: CurrentLevel];

        private void OnEnable()
        {
            GameEconomy.OnMoneyChange += CheckInteractability;
        }

        private void OnDisable()
        { 
            GameEconomy.OnMoneyChange -= CheckInteractability;
        }
        
        private void Start()
        {
            Button.onClick.AddListener(OnClick);
            CheckInteractability();
        }

        private void OnClick()
        {
            if (GameEconomy.Instance.HasEnoughMoney(CurrentPrice))
            {
                GameEconomy.Instance.SpendMoney(CurrentPrice);
                OnBought();
                SuccessAnim();
            }
            else
            {
                FailAnim();
            }
        }

        protected virtual void OnBought()
        {
            onBought?.Invoke();
        }
        
        private void CheckInteractability()
        {
            overlay.SetActive(!GameEconomy.Instance.HasEnoughMoney(CurrentPrice));
        }
        
        private void SuccessAnim()
        {
            transform.DOComplete();
            transform.DOScale(0.8f, 0.2f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
        }

        private void FailAnim()
        {
            transform.DOKill();
            transform.rotation = Quaternion.Euler(Vector3.zero);
            var rot = Vector3.zero;
            rot.z -= 15;
            transform.DORotate(rot, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                rot.z += 30;
                transform.DORotate(rot, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            });
        }
    }
}
