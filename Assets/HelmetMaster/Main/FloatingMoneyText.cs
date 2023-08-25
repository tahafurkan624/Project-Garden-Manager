using DG.Tweening;
using TMPro;
using UnityEngine;

namespace HelmetMaster.Main
{
    public class FloatingMoneyText : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyTMPText;

        public void Init()
        {
            gameObject.SetActive(false);
        }

        public void PlayFloatingMoney(string textString, Vector3 pos, float YMove, float duration)
        {
            moneyTMPText.text = textString;
            transform.DOKill();
            transform.position = pos;
            transform.DOMoveY(pos.y + YMove, duration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}