using System;
using System.Collections;
using _Main._Scripts.Utilities;
using HelmetMaster.Main;
using HelmetMaster.Main.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main._Scripts.Managers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        public static event Action<GameState> OnBeforeStateChanged;
        public static event Action<GameState> OnAfterStateChanged;

        public GameState State { get; private set; }

        public bool initialHelmetMaster = true;

        public LevelManagerSO levelManagerSo;

        protected override void Awake()
        {
            base.Awake();

            if (levelManagerSo != null)
            {
                var managerSetup = levelManagerSo.GameManagerSettings;

                if (managerSetup.SplashScene)
                {
                    managerSetup.SplashScene = false;
                }
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if(scene.buildIndex == 0) return;

            ChangeState(GameState.Starting);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        IEnumerator Start()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (initialHelmetMaster)
            {
                yield return new WaitForSeconds(levelManagerSo.GameManagerSettings.MinWaitTime);

                for (int i = 0; i < levelManagerSo.TutorialScenes.Count; i++)
                {
                    if (GlobalPlayerPrefs.GetTutorialScenePlayed(i+1)) continue;
                    yield return LoadAsyncTutorialScene(i);
                    yield break;
                }
                yield return LoadAsyncScene(GlobalPlayerPrefs.ReachedLevel);
            }
            else
            {
                initialHelmetMaster = true;
            }
        }

        private const float Min = 60f;
        private float _lastTime;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ChangeState(GameState.Win);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeState(GameState.Lose);
            }

            if (Time.time > _lastTime + Min)
            {
                _lastTime = Time.time;
                GlobalPlayerPrefs.ContinuousPlayedMinutes++;
            }
        }

        public void ChangeState(GameState newState) {
            OnBeforeStateChanged?.Invoke(newState);

            State = newState;
            switch (newState) {
                case GameState.Starting:
                    HandleStarting();
                    break;
                case GameState.Playing:
                    break;
                case GameState.Win:

                    var sceneIdx = SceneManager.GetActiveScene().buildIndex;
                    var levelCount = levelManagerSo.LevelScenes.Count;
                    if (sceneIdx > levelCount)
                    {
                        var tutIdx = sceneIdx - levelManagerSo.LevelScenes.Count;
                        GlobalPlayerPrefs.SetTutorialScenePlayed(tutIdx, true);
                        // Send Negative TutIdx Success
                    }
                    else
                    {
                        GlobalPlayerPrefs.ReachedLevel++;
                        // Send ReachedLevel Success
                    }

                    MainCanvas.Instance.EnableSuccessUI();
                    break;
                case GameState.Lose:
                    MainCanvas.Instance.EnableFailUI();
                    break;
            }

            OnAfterStateChanged?.Invoke(newState);
        }

        private void HandleStarting() {
            // Do some start setup, could be environment, cinematics etc

            // Eventually call ChangeState again with your next state

            if (MainCanvas.Instance.TapToStartActivity)
            {
                MainCanvas.Instance.EnableTapToStartUI();
            }
            else
            {
                ChangeState(GameState.Playing);
            }
        }

        public void LoadLevel(int index)
        {
            StartCoroutine(LoadAsyncScene(index));
        }

        public void LoadReachedLevel()
        {
            StartCoroutine(LoadAsyncScene(GlobalPlayerPrefs.ReachedLevel));
        }

        public void LoadCurrentLevel()
        {
            StartCoroutine(LoadAsyncScene(GlobalPlayerPrefs.CurrentLevel));
        }

        private IEnumerator LoadAsyncTutorialScene(int tutorialIdx)
        {
            yield return SceneManager.LoadSceneAsync(levelManagerSo.LevelScenes.Count + 1 + tutorialIdx, LoadSceneMode.Single);
        }

        private IEnumerator LoadAsyncScene(int sceneNumber)
        {
            yield return SceneManager.LoadSceneAsync(GetSceneIdx(sceneNumber), LoadSceneMode.Single);

            // yield return SceneManager.LoadSceneAsync(GetSceneName(sceneNumber), LoadSceneMode.Single);

            GlobalPlayerPrefs.CurrentLevel = sceneNumber;

            // string GetSceneName(int sceneIndex)
            // {
            //     var levelCount = levelManagerSo.LevelScenes.Count;
            //     sceneIndex %= levelCount;
            //     return levelManagerSo.LevelScenes[sceneIndex == 0 ? levelCount-1 : sceneIndex-1].Name;
            // }

            int GetSceneIdx(int sceneIndex)
            {
                var levelCount = levelManagerSo.LevelScenes.Count;
                sceneIndex %= levelCount;
                return sceneIndex == 0 ? levelCount : sceneIndex;
                //return levelManagerSo.LevelScenes[].Name;
            }
        }

        public bool TryLoadTutorialScene()
        {
            for (int i = 0; i < levelManagerSo.TutorialScenes.Count; i++)
            {
                if (GlobalPlayerPrefs.GetTutorialScenePlayed(i+1)) continue;
                StartCoroutine(LoadAsyncTutorialScene(i));
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public enum GameState {
        Starting = 0,
        Playing = 1,
        Win = 2,
        Lose = 3,
    }
}