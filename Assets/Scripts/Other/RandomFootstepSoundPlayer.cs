using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class RandomFootstepSoundPlayer : MonoBehaviour
	{
		public AudioClip[] footstepClips;
		public float footStepsInterval = 0.3f;
        public float footStepsStartTime = 0.5f;
        private float startFootstepTime;
        private int maxFootstepCount;
		private AudioSource audioSource;

        private void Awake()
        {
			audioSource = GetComponent<AudioSource>();
		}

        private void OnEnable()
        {
            footStepsStartTime = 0.5f;
            maxFootstepCount = 0;
        }

        private void Update()
        {
            if (maxFootstepCount >= 3) return;
            footStepsStartTime -= Time.deltaTime;
            if (footStepsStartTime > 0) return;

            if (startFootstepTime < Time.time)
            {
                if (audioSource != null)
                {
                    audioSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.volume = Random.Range(0.35f, 0.45f);
                    audioSource.Play();
                    maxFootstepCount++;
                }
                startFootstepTime = Time.time + footStepsInterval;
            }
        }
    }
}