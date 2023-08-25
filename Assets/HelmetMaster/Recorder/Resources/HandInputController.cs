using UnityEngine; //using DG.Tweening;

namespace zz_HelmetMaster.Recorder.Resources
{
    public class HandInputController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) Tap();
            if (Input.GetMouseButtonUp(0)) UnTap();
        }

        private void Tap()
        {
            //transform.DOComplete();
            //transform.DOScale(Vector3.one * .75f, .25f);
        }

        private void UnTap()
        {
            //transform.DOComplete();
            //transform.DOScale(Vector3.one, .25f);
        }
    }
}
