using HelmetMaster.Main.UI;
using MoreMountains.NiceVibrations;

namespace HelmetMaster.Main
{
    public static class VibrationManager
    {
        public static void Haptic(HapticTypes hapticType)
        {
            //if (!SettingsUI.VibrationEnabled) return;

            MMVibrationManager.Haptic(hapticType, false, false);
        }
    }
}