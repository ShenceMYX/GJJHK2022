using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class DoorDetector : MonoBehaviour
	{
        //private TilemapSwapper.Entity entityType;

        private void OnEnable()
        {
            //entityType = GetComponent<CharacterInputController>().entityType;
            GetComponent<CharacterInputController>().isAboutToMoveHandler += CheckDoor;
        }


        private bool CheckDoor(TilemapSwapper.Entity entityType, Vector2 direction)
        {
            TilemapSwapper.Entity otherEntityType = entityType == TilemapSwapper.Entity.A ? TilemapSwapper.Entity.B : TilemapSwapper.Entity.A;
            //Debug.Log(TilemapSwapper.Instance.GetOffsetTileType(entityType, new Vector2Int((int)direction.x, (int)direction.y)));
            if (TilemapSwapper.Instance.GetOffsetTileType(entityType, new Vector2Int((int)direction.x, (int)direction.y)) == TilemapSwapper.TileType.DOOR)
            {
                if (TilemapSwapper.Instance.GetOffsetTileType(otherEntityType, new Vector2Int((int)direction.x, (int)direction.y)) != TilemapSwapper.TileType.DOOR)
                {
                    MenuController.Instance.ShowWaitingUI();
                    return false;
                }
                else
                {
                    //在地图未加载完（转场动画未播放完）之前不让玩家乱操作
                    //PlayerInstance.Instance.canInput = false;
                    MenuController.Instance.CloseWaitingUI();

                    MenuController.Instance.PlaySceneTransition(direction);
                    LevelManager.Instance.LoadNextLevel(transform.position, direction);
                    return true;
                }

            }
            else
            {
                MenuController.Instance.CloseWaitingUI();
                return true;
            }
        }

        private void OnDisable()
        {
            GetComponent<CharacterInputController>().isAboutToMoveHandler -= CheckDoor;
        }
    }
}