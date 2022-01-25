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
        private Vector2 initialPos;

        private void Awake()
        {
            initialPos = transform.position;    
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

            transform.position = pos;
		}

        public void ResetPos()
        {
            transform.position = initialPos;
        }

        void OnDrawGizmos()
		{
			Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
			Gizmos.DrawLine(colliderCenterPos, new Vector2(colliderCenterPos.x - 1, colliderCenterPos.y));
		}
	}

}