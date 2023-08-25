using UnityEngine;
using UnityEngine.UI;

namespace HelmetMaster.Main.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private Button audioButton, vibrationButton;

        public static bool AudioEnabled, VibrationEnabled;

        private void Awake()
        {
            audioButton.onClick.AddListener(SwitchAudio);
            vibrationButton.onClick.AddListener(SwitchVibration);

            AudioEnabled = GlobalPlayerPrefs.AudioEnabled;
            VibrationEnabled = GlobalPlayerPrefs.VibrationEnabled;

            if (AudioEnabled) { EnableAudio(); }
            else { DisableAudio(); }
            if (VibrationEnabled) { EnableVibration(); }
            else { DisableVibration(); }
        }

        private void SwitchAudio()
        {
            if (AudioEnabled)
            {
                DisableAudio();
            }
            else
            {
                EnableAudio();
            }
        }

        private void EnableAudio()
        {
            AudioEnabled = true;
            GlobalPlayerPrefs.AudioEnabled = true;
        }

        private void DisableAudio()
        {
            AudioEnabled = false;
            GlobalPlayerPrefs.AudioEnabled = false;
        }

        private void SwitchVibration()
        {
            if (VibrationEnabled)
            {
                DisableVibration();
            }
            else
            {
                EnableVibration();
            }
        }

        private void EnableVibration()
        {
            VibrationEnabled = true;
            GlobalPlayerPrefs.VibrationEnabled = true;
        }

        private void DisableVibration()
        {
            VibrationEnabled = false;
            GlobalPlayerPrefs.VibrationEnabled = false;
        }
    }
}
