using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class CharacterMotor : MonoBehaviour
	{
        public float walkSpeed = 5f;
        public Vector3 target;
        private Animator anim;
        public bool reachTarget = true;

        public event Action onArrivalHandler;

        public AudioClip[] footstepClips;
        public float footStepsInterval = 0.3f;
        private float startFootstepTime;
        private AudioSource audioSource;

        public ParticleSystem footStepDustParticle;
        private ParticleSystem.EmissionModule footStepDustParticleEmission;
        private ParticleSystem.MinMaxCurve currentEmissionRate;

        private void Awake()
        {
            target = transform.position;
            anim = GetComponentInChildren<Animator>();

            audioSource = GetComponent<AudioSource>();

            footStepDustParticleEmission = footStepDustParticle.emission;
            currentEmissionRate = footStepDustParticleEmission.rateOverTime;
            footStepDustParticleEmission.rateOverTime = 0;
        }

        private void Update()
        {
            reachTarget = Vector3.Distance(target, transform.position) < 0.01f;
        }

        public void Movement(Vector2 dir)
        {
            if (PlayerInstance.Instance.cantMove) return;
            RaycastHit2D ray;
			Vector2 pos = new Vector2(transform.position.x, transform.position.y);
			Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.25f);

            ray = Physics2D.Linecast(colliderCenterPos, new Vector2(colliderCenterPos.x, colliderCenterPos.y) + dir);
            if (!ray.collider)
            {
                pos += dir;
            }

            target = pos;

            if(!ray.collider)
                StartCoroutine(MoveTowardTarget());

        }

        private IEnumerator MoveTowardTarget()
        {
            anim.SetBool("walk", true);
            reachTarget = false;
            while (!reachTarget)
            {
                footStepDustParticleEmission.rateOverTime = currentEmissionRate;
                PlayeFootstepSound();
                Vector3 tar = Vector3.MoveTowards(transform.position, target, walkSpeed * Time.deltaTime);
                transform.position = tar;
                yield return null;
            }
            transform.position = target;
            anim.SetBool("walk", false);
            onArrivalHandler?.Invoke();
            footStepDustParticleEmission.rateOverTime = 0;
        }

        private void PlayeFootstepSound()
        {
            if (startFootstepTime < Time.time)
            {
                if (audioSource != null)
                {
                    audioSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.volume = Random.Range(0.35f, 0.45f);
                    audioSource.Play();
                }
                startFootstepTime = Time.time + footStepsInterval;
            }
        }

        public void ResetPos(Vector3 pos)
        {
            transform.position = pos;
            target = pos;
        }
        
    }

}