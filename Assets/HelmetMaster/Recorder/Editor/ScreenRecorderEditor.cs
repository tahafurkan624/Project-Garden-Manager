
using HelmetMaster.Recorder.Runtime;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

#endif

namespace HelmetMaster.Recorder.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ScreenRecorder))]
    [CanEditMultipleObjects]
    public class ScreenRecorderEditor : UnityEditor.Editor
    {
        private Rect screenRect;
        private Rect vertRect;

        private ScreenRecorder screenRecorder;

        private void Awake()
        {
            screenRecorder = ScreenRecorder.Instance;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space(5);

            // if (screenRecorder.IsCustomResolution)
            // {
            //     if (screenRecorder.ResolutionData != null)
            //     {
            //         screenRecorder.ResolutionData.outputWidth = EditorGUILayout.IntField("Width",screenRecorder.ResolutionData.outputWidth);
            //         screenRecorder.ResolutionData.outputHeight = EditorGUILayout.IntField("Height",screenRecorder.ResolutionData.outputHeight);
            //     }
            // }
        
            screenRect = GUILayoutUtility.GetRect(1, 1);
            vertRect = EditorGUILayout.BeginVertical();

            if (Application.isPlaying)
            {
                if (screenRecorder != null)
                {
                    if (screenRecorder.IsDeviceSimulatorRunning == false)
                    {
                        if (screenRecorder.IsRecording == true)
                        {
                            EditorGUI.DrawRect(new Rect(screenRect.x - 13, screenRect.y - 1, screenRect.width + 25, vertRect.height + 14), Color.red);
                            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                            EditorGUILayout.LabelField("Recording...", style);
                            Repaint();
                        }
                        else
                        {
                            EditorGUI.DrawRect(new Rect(screenRect.x - 13, screenRect.y - 1, screenRect.width + 25, vertRect.height + 14), Color.green);
                            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                            EditorGUILayout.LabelField("Ready for recording.", style);
                            Repaint();
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Please switch to Gameview. Recorder can not work with Simulator.", MessageType.Error);
                        Repaint();
                    }
                
                }               
            }
            else
            {
                EditorGUI.DrawRect(new Rect(screenRect.x - 13, screenRect.y - 1, screenRect.width + 25, vertRect.height + 14), Color.yellow);
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                EditorGUILayout.LabelField("Waiting for play mode...", style);
            }
        
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Open Recordings Folder"))
            {
                if (!Directory.Exists("Recordings"))
                {
                    Debug.LogError($"{GetType().Name} -> Recordings folder is empty! Please record something first.");
                }
                else
                {
                    EditorUtility.RevealInFinder("Recordings");
                }
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("How to use?");
            EditorGUILayout.HelpBox("Press 0 to take screenshots while game is running", MessageType.Info);
            EditorGUILayout.HelpBox("Press 8 to open/close hand icon", MessageType.Info);
            EditorGUILayout.HelpBox("Press 9 to start/stop video recording while game is running", MessageType.Info);
        }
    }
#endif
}