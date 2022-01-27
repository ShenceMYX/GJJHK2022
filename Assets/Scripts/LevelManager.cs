using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class LevelManager : MonoSingleton<LevelManager>
	{
		private Transform[] allLevelsTrans;
		public float raycastDistance = 20f;
		public LayerMask levelLayer;

        private void Start()
        {
			allLevelsTrans = new Transform[transform.childCount];
            for (int i = 0; i < allLevelsTrans.Length; i++)
            {
				allLevelsTrans[i] = transform.GetChild(i);
            }

            LoadNextLevel(new Vector3(-3.4f, 0, 0), new Vector2(1, 0));
        }

        public void LoadNextLevel(Vector3 pos, Vector2 dir)
        {
            foreach (var levelTF in allLevelsTrans)
            {
                levelTF.GetComponent<BoxCollider2D>().enabled = true;
            }

            //找到最远的射线检测碰撞器（除了自己的另外一个）
            RaycastHit2D[] rays = Physics2D.LinecastAll(pos, new Vector2(pos.x, pos.y) + dir*raycastDistance, levelLayer);
			RaycastHit2D furthestRay = rays.GetMax(t => Vector3.Distance(t.collider.transform.position, pos));

			Transform bornPointsFather = furthestRay.collider.transform.Find("BornPoints");

            //重新加载地图
            Grid grid = furthestRay.collider.transform.GetComponentInChildren<Grid>();

            TilemapSwapper.Instance.SelectTilemaps(grid);
			//TilemapSwapper.Instance.SelectTilemapData(furthestRay.collider.transform.GetComponentInChildren<TilemapData>());


			//找到离玩家最近的出生点
			Vector3 nearestBornPoint = bornPointsFather.GetChild(0).position;
            for (int i = 0; i < bornPointsFather.childCount; i++)
            {
				if(Vector3.Distance(bornPointsFather.GetChild(i).position, pos) < Vector3.Distance(nearestBornPoint, pos))
                {
					nearestBornPoint = bornPointsFather.GetChild(i).position;
                }
            }

			//初始化地图和玩家位置
			MapController.Instance.playerInitialPos = nearestBornPoint;
			MapController.Instance.ResetMapAndPlayerPos();
            

            foreach (var levelTF in allLevelsTrans)
            {
                levelTF.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
	}
}