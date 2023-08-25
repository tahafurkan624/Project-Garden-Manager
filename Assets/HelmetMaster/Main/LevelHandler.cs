using System.Linq;
using _Main._Scripts.Managers;
using _Main._Scripts.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HelmetMaster.Main
{
    public class LevelHandler : Singleton<LevelHandler>
    {
        [SerializeField] private LevelManagerSO levelsSo;

        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            if (GameManager.Instance == null)
            {
                if (!IsSceneInProject(SceneManager.GetActiveScene().name))
                {
                    var gameManager = gameObject.AddComponent<GameManager>();
                    gameManager.initialHelmetMaster = false;
                    gameManager.levelManagerSo = levelsSo;
                    GlobalPlayerPrefs.CurrentLevel = SceneManager.GetActiveScene().buildIndex;
                    gameManager.ChangeState(GameState.Starting);
                    Destroy(this);
                }
                else
                {
                    GlobalPlayerPrefs.ReachedLevel = SceneManager.GetActiveScene().buildIndex;
                    SceneManager.LoadScene(0);
                }
            }
            else
            {
                Destroy(gameObject);
            }
#endif
        }
#if UNITY_EDITOR
        public static bool IsSceneInProject(string named)
        {
            return EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path.Contains("/" + named + ".unity"));
        }
#endif
    }
}