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

            LoadNextLevel(new Vector3(20-3.4f, 2, 0), new Vector2(1, 0));
        }

        private Vector3 p;
        private Vector2 d;
        public void LoadNextLevel(Vector3 pos, Vector2 dir)
        {
            p = pos; d = dir;
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

            //设置Cinemachine新房间的边界
            CinemachineManager.Instance.ResetVCamsConfiner(grid.GetComponent<PolygonCollider2D>());

			//找到离玩家最近的出生点
			Vector3 nearestBornPoint = bornPointsFather.GetChild(0).position;
            for (int i = 0; i < bornPointsFather.childCount; i++)
            {
				if(Vector3.Distance(bornPointsFather.GetChild(i).position, pos) < Vector3.Distance(nearestBornPoint, pos))
                {
					nearestBornPoint = bornPointsFather.GetChild(i).position;
                }
            }

            //StartCoroutine(WaitInializePlayerPos(nearestBornPoint));
            MapController.Instance.playerInitialPos = nearestBornPoint;

            CinemachineManager.Instance.TransformVCameras(dir * 20);


            foreach (var levelTF in allLevelsTrans)
            {
                levelTF.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private IEnumerator WaitInializePlayerPos(Vector3 nearestBornPoint)
        {
            yield return new WaitForEndOfFrame();
            //初始化地图和玩家位置
           

        }

        void OnDrawGizmos()
        {
            //Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
            //Gizmos.DrawLine(colliderCenterPos, new Vector2(colliderCenterPos.x - 1, colliderCenterPos.y));
            Vector2 colliderCenterPos = new Vector2(transform.position.x, transform.position.y);
            Gizmos.DrawLine(p, new Vector2(p.x,p.y)+d*20);
        }
    }
}