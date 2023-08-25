using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class SeedAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Seed seed;
        
        public void OnGrown()
        {
            seed.OnGrown();
        }
    }
}