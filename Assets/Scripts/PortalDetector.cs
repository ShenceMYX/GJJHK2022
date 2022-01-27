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

        private void OnEnable()
        {
			entityType = GetComponent<CharacterInputController>().entityType;
			GetComponent<CharacterMotor>().onArrivalHandler += CheckPortal;
        }

        private void CheckPortal()
		{
			if (TilemapSwapper.Instance.GetCurrentTileType(entityType) == TilemapSwapper.TileType.PORTAL)
			{
				transform.position = new Vector3(TilemapSwapper.Instance.GetOtherPortalLocation(entityType).x + 0.6f, TilemapSwapper.Instance.GetOtherPortalLocation(entityType).y + 1, 0);
			}
		}

		private void OnDisable()
		{
			GetComponent<CharacterMotor>().onArrivalHandler -= CheckPortal;
		}

	}
}