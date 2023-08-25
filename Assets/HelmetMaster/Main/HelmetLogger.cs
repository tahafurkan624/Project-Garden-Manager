using System.Collections.Generic;
using UnityEngine;

namespace HelmetMaster.Main
{
    public static class HelmetLogger
    {
        public static bool IsLogging = true;

        public static void LogInEditor(string text)
        {
            if(!IsLogging) return;

#if UNITY_EDITOR
            Debug.Log(text);
#endif
        }

        public static void Log(string text)
        {
            if(!IsLogging) return;

            Debug.Log(text);
        }

        public static void LogList<T>(ref List<T> list, bool onlyInEditor = true)
        {
            if(!IsLogging) return;

            if (onlyInEditor)
            {
                foreach (var t in list)
                {
                    Debug.Log(t.ToString());
                }
            }
            else
            {
#if UNITY_EDITOR
                foreach (var t in list)
                {
                    Debug.Log(t.ToString());
                }
#endif
            }
        }

        public static void LogArray<T>(ref T[] array, bool onlyInEditor)
        {
            if(!IsLogging) return;

            if (onlyInEditor)
            {
                foreach (var t in array)
                {
                    Debug.Log(t.ToString());
                }
            }
            else
            {
#if UNITY_EDITOR
                foreach (var t in array)
                {
                    Debug.Log(t.ToString());
                }
#endif
            }
        }
    }
}