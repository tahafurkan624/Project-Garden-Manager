using System;
using System.Collections.Generic;
using _Main._Scripts.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace HelmetMaster.Main
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private int AudioSourceCount = 5;

        private List<AudioSource> _audioSources = new List<AudioSource>();

        private bool _audioManagerInitialized, _generateAs;

        private int _lastUsedAsIdx = 0;

        protected override void Awake()
        {
            base.Awake();
            CheckInit();
        }

        private void CheckInit()
        {
            if (!_audioManagerInitialized) { Init(); }
        }

        private void Init()
        {
            for (int i = 0; i < AudioSourceCount; i++)
            {
                var aS = GenerateAudioSource();
            }

            _generateAs = _audioSources.Count < 1;

            _audioManagerInitialized = true;
        }

        public void PlayClip(AudioClip audioClip)
        {
            if(!GlobalPlayerPrefs.AudioEnabled) return;

            CheckInit();

            var aS = PickAudioSource(_generateAs);
            aS.PlayOneShot(audioClip);
        }

        public void PlayClipWithPitch(AudioClip audioClip, float pitch)
        {
            if(!GlobalPlayerPrefs.AudioEnabled) return;

            CheckInit();

            var aS = PickAudioSource(_generateAs);
            aS.pitch = pitch;
            aS.PlayOneShot(audioClip);
        }

        public float CalculatePitch(float minPitch, float maxPitch, int i, int maxI)
        {
            var t = Mathf.InverseLerp(0, maxI, i);
            return Mathf.Clamp(t, minPitch, maxPitch);
        }

        private AudioSource PickAudioSource(bool shouldGenerateAs)
        {
            var idx = _lastUsedAsIdx;
            _lastUsedAsIdx++;
            idx = Mathf.Clamp(idx, 0, _audioSources.Count - 1);
            var aS = _audioSources[idx];
            if (shouldGenerateAs) if (aS.isPlaying) aS = GenerateAudioSource();
            return aS;
        }

        private AudioSource GenerateAudioSource()
        {
            var aS = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(aS);
            return aS;
        }
    }
}
