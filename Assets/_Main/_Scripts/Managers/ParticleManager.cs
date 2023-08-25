using System;
using System.Collections.Generic;
using _Main._Scripts.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.Managers
{
    public class ParticleManager : Singleton<ParticleManager>
    {
        [SerializeField] private List<HelmetParticle> particles;

        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }

        public ParticleSystem PlayParticle(ParticleTag tag, Vector3 pos, Quaternion rotation = new Quaternion())
        {
            var particlesWithThisTag = particles.FindAll(particle => particle.tag == tag);
            if (particlesWithThisTag.Count < 1)
            {
                Debug.LogError("Helmet Particle Manager could not find particle with this tag: " + tag.ToString());
                return null;
            }
            var helmetParticle = particlesWithThisTag[Random.Range(0, particlesWithThisTag.Count)];
            var ps = helmetParticle.GetParticle();
            ps.transform.position = pos;
            ps.transform.rotation = rotation;
            ps.gameObject.SetActive(true);
            if(ps.isPlaying) ps.Stop();
            ps.Play();
            return ps;
        }

        private void InitializePools()
        {
            foreach (var helmetParticle in particles)
            {
                helmetParticle.Init();
            }
        }
    }

    [Serializable]
    public class HelmetParticle
    {
        public ParticleSystem particlePrefab;
        public ParticleTag tag;
        public int maxAmount;
        [Tooltip("0: Kills when its completed\n-1:Never Kills\n>0:Kills after given duration")]
        public float duration;
        public bool loop;

        private int lastIndex = 0;
        private LimitedParticlePool pool = new LimitedParticlePool();

        public void Init()
        {
            ParticleSystemStopAction stopAction;
            if (Math.Abs(duration - (-1)) < .001)
            {
                stopAction = ParticleSystemStopAction.None;
            }
            else if (Math.Abs(duration) < .001)
            {
                stopAction = ParticleSystemStopAction.Disable;
            }
            else
            {
                duration = Mathf.Abs(duration);
                stopAction = ParticleSystemStopAction.Disable;
            }
            pool.Init(maxAmount, particlePrefab, stopAction, loop);
        }

        public ParticleSystem GetParticle()
        {
            int idx = lastIndex;
            lastIndex++;
            if (lastIndex >= maxAmount) lastIndex = 0;
            return pool.GetParticle(idx);
        }
    }

    public class LimitedParticlePool : MonoBehaviour
    {
        private ParticleSystem[] particles;

        public void Init(int amount, ParticleSystem ps, ParticleSystemStopAction stopAction, bool loop)
        {
            particles = new ParticleSystem[amount];
            for (var i = 0; i < particles.Length; i++)
            {
                //var particle = PrefabUtility.InstantiatePrefab(ps) as ParticleSystem;
                var particle = Instantiate(ps);
                particle.gameObject.SetActive(false);
                particle.transform.SetParent(ParticleManager.Instance.transform);
                var particleMain = particle.main;
                particleMain.stopAction = stopAction;
                particleMain.loop = loop;
                particles[i] = particle;
            }
        }

        public ParticleSystem GetParticle(int index)
        {
            return particles[index];
        }
    }

    public enum ParticleTag
    {
        SeedBombing,
        Growing,
        Grown,
        Tag3,
        Tag4,
        Tag5,
        Tag6,
        Tag7,
        Tag8,
        Tag9,
    }
}