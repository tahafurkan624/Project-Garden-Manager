using System.Collections;
using System.Collections.Generic;
using _Main._Scripts._General.FarmingSystem;
using _Main._Scripts.Utilities;
using UnityEngine;

namespace _Main._Scripts.Managers
{
    public class FarmAreasManager : Singleton<FarmAreasManager>
    {
        [SerializeField] private List<FarmArea> farmAreas = new List<FarmArea>();
        [SerializeField] private float duration;
        
        private bool isBoosted;
        
        public void BoostSprinkles(SprinklerBoostButton button)
        {
            if (isBoosted) return;

            isBoosted = true;
            foreach (var farmArea in farmAreas)
            {
                farmArea.SpeedUpRows();
            }

            StartCoroutine(SpeedUpForDuration(button));
        }

        IEnumerator SpeedUpForDuration(SprinklerBoostButton button)
        {
            yield return new WaitForSeconds(duration);
            foreach (var farmArea in farmAreas)
            {
                farmArea.ResetRowSpeeds();
            }

            isBoosted = false;
            yield return new WaitForSeconds(60f);
            button.Activate();
        }
    }
}