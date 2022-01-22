using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class MapController : MonoBehaviour
	{
        private bool isCurrentTileMapA = false;

        private void OnEnable()
        {
            TilemapSwapper.Instance.InitializeTilemap();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isCurrentTileMapA)
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.A);
                else
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.B);
                isCurrentTileMapA = !isCurrentTileMapA;
            }

        }
    }
}