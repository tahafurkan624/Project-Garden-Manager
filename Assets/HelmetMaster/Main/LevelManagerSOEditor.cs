#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HelmetMaster.Main
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelManagerSO))]
    public class LevelManagerSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (LevelManagerSO)target;
 
            if(GUILayout.Button("Auto Adjust Scene Builds", GUILayout.Height(40)))
            {
                script.SetScenesToBuild();
            }
        }
    }
#endif
}