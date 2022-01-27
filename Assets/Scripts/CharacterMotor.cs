using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private void Awake()
        {
            target = transform.position;
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            reachTarget = Vector3.Distance(target, transform.position) < 0.01f;
        }

        public void Movement(Vector2 dir)
        {
            RaycastHit2D ray;
			Vector2 pos = new Vector2(transform.position.x, transform.position.y);
			Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.25f);

            ray = Physics2D.Linecast(colliderCenterPos, new Vector2(colliderCenterPos.x, colliderCenterPos.y) + dir);
            if (!ray.collider)
            {
                pos += dir;
            }

            target = pos;
            StartCoroutine(MoveTowardTarget());

            onArrivalHandler?.Invoke();
        }

        private IEnumerator MoveTowardTarget()
        {
            anim.SetBool("walk", true);
            reachTarget = false;
            while (!reachTarget)
            {
                Vector3 tar = Vector3.MoveTowards(transform.position, target, walkSpeed * Time.deltaTime);
                transform.position = tar;

                yield return null;
            }
            transform.position = target;
            anim.SetBool("walk", false);
        }

        public void ResetPos(Vector3 pos)
        {
            transform.position = pos;
            target = pos;
        }

        void OnDrawGizmos()
		{
            //Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
            //Gizmos.DrawLine(colliderCenterPos, new Vector2(colliderCenterPos.x - 1, colliderCenterPos.y));
            Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y);
            Gizmos.DrawLine(colliderCenterPos, new Vector2(colliderCenterPos.x + 20, colliderCenterPos.y));
        }
    }

}