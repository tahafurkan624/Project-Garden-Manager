using UnityEngine;

//[CreateAssetMenu(fileName = "RecorderData", menuName = "ScriptableObjects/RecorderData", order = 3)]
namespace zz_HelmetMaster.Recorder.Resources
{
    public class RecorderData : ScriptableObject
    {
        public int screenshotTakeCount = 0;
        public int videoTakeCount = 0;
    }
}
