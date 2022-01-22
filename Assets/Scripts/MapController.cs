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
        private GameObject[] playerGOs;

        private void OnEnable()
        {
            TilemapSwapper.Instance.InitializeTilemap();

            playerGOs = new GameObject[2];
            int index = 0;
            foreach (var player in FindObjectsOfType<CharacterInputController>())
            {
                playerGOs[index++] = player.gameObject;
            }
            playerGOs[0].SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isCurrentTileMapA)
                {
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.A);
                    playerGOs[0].SetActive(true);
                    playerGOs[1].SetActive(false);
                }
                else
                {
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.B);
                   
                    playerGOs[0].SetActive(false);
                    playerGOs[1].SetActive(true);
                }
                isCurrentTileMapA = !isCurrentTileMapA;
            }

        }
    }
}