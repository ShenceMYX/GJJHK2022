using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class RoomTransitionTrigger : MonoBehaviour
	{
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("?????????");
                TilemapSwapper.Instance.SelectTilemaps(transform.root.GetComponent<Grid>(), TilemapSwapper.Entity.A);
            }
        }
    }
}