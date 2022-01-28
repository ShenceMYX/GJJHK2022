using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Common;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class CinemachineSwitcher : MonoSingleton<CinemachineSwitcher>
	{
		public CinemachineVirtualCamera vcamA;
		public CinemachineVirtualCamera vcamB;

		public void SwitchVCamPriority(bool isCurrentTilemapA)
        {
            if (isCurrentTilemapA)
            {
                vcamA.Priority = 1;
                vcamB.Priority = 0;
            }
            else
            {
                vcamA.Priority = 0;
                vcamB.Priority = 1;
            }
        }

		public void TransformVCameras(Vector3 transPos)
        {
			vcamB.transform.Translate(transPos);
			vcamA.transform.Translate(transPos);
		}

        public void ResetVCamsConfiner(PolygonCollider2D roomCollider)
        {
             vcamA.GetComponent<CinemachineConfiner>().m_BoundingShape2D = roomCollider;
             vcamB.GetComponent<CinemachineConfiner>().m_BoundingShape2D = roomCollider;
        }
    }
}