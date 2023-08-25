using System;
using UnityEngine;

namespace HelmetMaster.Main
{
    public static class GlobalPlayerPrefs
    {
        public static int CurrentLevel
        {
            get => PlayerPrefs.GetInt("CurrentLevel", 1);
            set => PlayerPrefs.SetInt("CurrentLevel", value);
        }

        public static int ReachedLevel
        {
            get => PlayerPrefs.GetInt("ReachedLevel", 1);
            set => PlayerPrefs.SetInt("ReachedLevel", value);
        }

        public static bool GetTutorialScenePlayed(int idx)
        {
            return PlayerPrefs.GetInt("Tutorial" + idx + "Played", 0) == 1;
        }

        public static void SetTutorialScenePlayed(int idx, bool value)
        {
            PlayerPrefs.SetInt("Tutorial" + idx + "Played", value ? 1 : 0);
        }

        public static bool AudioEnabled
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt("AudioEnabled", 1));
            set => PlayerPrefs.SetInt("AudioEnabled", value ? 1 : 0);
        }

        public static bool VibrationEnabled
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt("VibrationEnabled", 1));
            set => PlayerPrefs.SetInt("VibrationEnabled", value ? 1 : 0);
        }

        public static int Money
        {
            get => PlayerPrefs.GetInt("MoneyInt", 0);
            set => PlayerPrefs.SetInt("MoneyInt", value);
        }

        public static int ContinuousPlayedMinutes
        {
            get => PlayerPrefs.GetInt("ContinuousPlayedMinutes", 0);
            set => PlayerPrefs.SetInt("ContinuousPlayedMinutes", value);
        }

        public static bool HasBoughtBigTruckBefore
        {
            get => PlayerPrefs.GetInt("HasBoughtBigTruckBefore", 0) == 1;
            set => PlayerPrefs.SetInt("HasBoughtBigTruckBefore", value ? 1 : 0);
        }

        public static float TimeDiscount => Mathf.Clamp(ContinuousPlayedMinutes/2f, 0, 10);

        public class SavedItems
        {
            public static int TotalCreateAmount     // Bought
            {
                get => PlayerPrefs.GetInt("TotalCreateAmount", 3);
                set => PlayerPrefs.SetInt("TotalCreateAmount", value);
            }

            public static int UnlockableCrateAmount
            {
                get => PlayerPrefs.GetInt("UnlockableCrateAmount", 4);
                set => PlayerPrefs.SetInt("UnlockableCrateAmount", value);
            }
        }
    }
}