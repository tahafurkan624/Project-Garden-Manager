using System;
using UnityEngine;

namespace _Main._Scripts._General.FarmingSystem
{
    public class Sprinkler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem sprayParticle;

        private void Start()
        {
            sprayParticle.Stop();
        }

        public void Sprinkle()
        {
            sprayParticle.Play();
        }

        public void SetSpeed()
        {
            
        }
    }
}