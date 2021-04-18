using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PII
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] AudioClip[] BackgroundMusicClips;

        private int clipIndex = 0;
        private AudioSource source { get { return GetComponent<AudioSource>(); } }

        private void Start()
        {
            PlayRandomClip();
        }

        private void Update()
        {
            if (source.isPlaying && source.clip)
            {
                if (source.time >= source.clip.length)
                {
                    PlayNextClip();
                }
            }
        }

        public void PlayNextClip()
        {
            clipIndex = clipIndex < BackgroundMusicClips.Length - 1 ? clipIndex + 1 : 0;
            PlayClip();
        }

        public void PlayPerviousClip()
        {
            clipIndex = clipIndex < BackgroundMusicClips.Length - 1 ? clipIndex - 1 : 0;
            PlayClip();
        }
        
        public void PlayRandomClip()
        {
            clipIndex = Random.Range(0,BackgroundMusicClips.Length);
            PlayClip();
        }

        public void StopPlaying()
        {
            source.time = 0;
            source.Stop();
        }

        private void PlayClip()
        {
            if (BackgroundMusicClips.Length < 1) return;

            StopPlaying();

            source.clip = BackgroundMusicClips[clipIndex];
            source.loop = false;
            source.Play();
        }
    }
}