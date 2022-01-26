using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class MapController : MonoSingleton<MapController>
	{
        private bool isCurrentTileMapA = true;
        private GameObject[] playerGOs;
        public Grid initialRoom;

        private void OnEnable()
        {
            TilemapSwapper.Instance.SelectTilemaps(initialRoom);
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

                    playerGOs[1].SetActive(true);
                    playerGOs[0].SetActive(false);
                    CheckIfOnWall(TilemapSwapper.Entity.A);
                }
                else
                {
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.B);
                   
                    playerGOs[1].SetActive(false);
                    playerGOs[0].SetActive(true);
                    CheckIfOnWall(TilemapSwapper.Entity.B);
                }
                isCurrentTileMapA = !isCurrentTileMapA;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ResetMapAndPlayerPos();
            }
        }

        public void ResetMapAndPlayerPos()
        {
            TilemapSwapper.Instance.InitializeTilemap();
            foreach (var player in playerGOs)
            {
                player.GetComponent<CharacterMotor>().ResetPos();

                player.GetComponent<CharacterInputController>().ResetFacingDir();
            }

            isCurrentTileMapA = true;

            playerGOs[1].SetActive(true);
            playerGOs[0].SetActive(false);
        }

        private void CheckIfOnWall(TilemapSwapper.Entity entityType)
        {
            if (TilemapSwapper.Instance.GetCurrentTileType(entityType) == TilemapSwapper.TileType.WALL)
                MenuController.Instance.PauseGame();
        }
    }
}