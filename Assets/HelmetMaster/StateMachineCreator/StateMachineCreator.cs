#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using zz_HelmetMaster.CreateUtils;
#endif

namespace zz_HelmetMaster.StateMachineCreator
{
    public static class StateMachineCreator
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/StateMachine/New StateMachine")]
        public static void CreateNewStateMachine()
        {
            string pathName = GetPathName();
            CreateNewScriptUtil.CreateScript(pathName);
        }
        
        private static string GetPathName()
        {
            string filePath;

            if (Selection.assetGUIDs.Length == 0)
                filePath = "Assets/NewSM.cs";
            else
                filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

            if (Directory.Exists(filePath))
            {
                filePath += "/NewSM.cs";
            }
            else
            {
                filePath = Path.GetDirectoryName(filePath) + "/NewSM.cs";
            }
            
            filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);

            return filePath;
        }
#endif
    }
}