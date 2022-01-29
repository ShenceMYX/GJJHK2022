using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class PortalDetector : MonoBehaviour
	{
		private TilemapSwapper.Entity entityType;
		private CharacterMotor motor;

        private void Awake()
        {
			entityType = GetComponent<CharacterInputController>().entityType;
			motor = GetComponent<CharacterMotor>();
		}

		private void OnEnable()
        {
			GetComponent<CharacterMotor>().onArrivalHandler += CheckPortal;
        }

        private void CheckPortal()
		{
			Debug.Log("onarrival");
			if (TilemapSwapper.Instance.GetCurrentTileType(entityType) == TilemapSwapper.TileType.PORTAL)
			{
				transform.position = new Vector3(TilemapSwapper.Instance.GetOtherPortalLocation(entityType).x + 0.6f, TilemapSwapper.Instance.GetOtherPortalLocation(entityType).y + 1, 0);
				motor.target = transform.position;
			}
		}

		private void OnDisable()
		{
			GetComponent<CharacterMotor>().onArrivalHandler -= CheckPortal;
		}

	}
}