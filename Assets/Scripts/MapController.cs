using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class MapController : MonoSingleton<MapController>
	{
        public bool isCurrentTileMapA { get; private set; } = true;
        private GameObject[] playerGOs;
        public Grid initialRoom;

        public Vector3 playerInitialPos { get; set; }

        public MMFeedbacks worldTransFeedbacks;

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

            playerInitialPos = playerGOs[0].transform.position;

            playerGOs[1].SetActive(false);
            playerGOs[0].SetActive(false);
            playerGOs[0].SetActive(true);
        }

        private void Update()
        {
            if (!PlayerInstance.Instance.canInput) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isCurrentTileMapA)
                {
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.A);

                    playerGOs[0].SetActive(true);
                    playerGOs[1].SetActive(false);
                    CheckIfOnWall(TilemapSwapper.Entity.A);
                }
                else
                {
                    TilemapSwapper.Instance.SwapTilemap(TilemapSwapper.Entity.B);
                   
                    playerGOs[0].SetActive(false);
                    playerGOs[1].SetActive(true);
                    CheckIfOnWall(TilemapSwapper.Entity.B);
                }
                isCurrentTileMapA = !isCurrentTileMapA;
                CinemachineManager.Instance.SwitchVCamPriority(isCurrentTileMapA);
                worldTransFeedbacks?.PlayFeedbacks();
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
                player.GetComponent<CharacterMotor>().ResetPos(playerInitialPos);

                player.GetComponent<CharacterInputController>().ResetFacingDir();
            }

            isCurrentTileMapA = true;

            playerGOs[0].SetActive(true);
            playerGOs[1].SetActive(false);
        }

        private void CheckIfOnWall(TilemapSwapper.Entity entityType)
        {
            if (TilemapSwapper.Instance.GetCurrentTileType(entityType) == TilemapSwapper.TileType.WALL)
                MenuController.Instance.PauseGame();
        }
    }
}