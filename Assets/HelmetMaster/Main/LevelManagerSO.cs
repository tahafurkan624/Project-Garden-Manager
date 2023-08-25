using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HelmetMaster.Main
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "LevelManagerSO", order = 0)]
    public class LevelManagerSO : ScriptableObject
    {
        public SceneReference SplashScene;
        public List<SceneReference> LevelScenes;
        public List<SceneReference> TutorialScenes;

        public GameManagerSettings GameManagerSettings;
#if UNITY_EDITOR
        List<SceneAsset> m_SceneAssets = new List<SceneAsset>();
#endif
        public void SetScenesToBuild()
        {
#if UNITY_EDITOR
            var scenesList = new List<EditorBuildSettingsScene>();

            var splashScene = new EditorBuildSettingsScene(SplashScene.ScenePath, true);

            scenesList.Add(splashScene);

            foreach (var t in LevelScenes)
            {
                var levelScene = new EditorBuildSettingsScene(t.ScenePath, true);
                scenesList.Add(levelScene);
            }

            foreach (var t in TutorialScenes)
            {
                var tutorialScene = new EditorBuildSettingsScene(t.ScenePath, true);
                scenesList.Add(tutorialScene);
            }

            EditorBuildSettings.scenes = scenesList.ToArray();
            
            // var newScene = new EditorBuildSettingsScene(buildScene.assetGUID, enabled);
            // var tempScenes = EditorBuildSettings.scenes.ToList();
            // tempScenes.Add(newScene);
            // EditorBuildSettings.scenes = tempScenes.ToArray();

            return;

            // Find valid Scene paths and make a list of EditorBuildSettingsScene
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            foreach (var sceneAsset in m_SceneAssets)
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (!string.IsNullOrEmpty(scenePath))
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
#endif
        }
    }

    [Serializable]
    public class GameManagerSettings
    {
        public bool SplashScene = true;
        public float MinWaitTime = .1f;
    }
}