using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class DoubleFlashlightController : MonoBehaviour
	{
		public LightShape singleLightShape;
		public LightShape doubleLightShape;

        private void OnEnable()
        {
            TilemapSwapper.Instance.SelectLightshape(TilemapSwapper.Entity.A, doubleLightShape);
            TilemapSwapper.Instance.SelectLightshape(TilemapSwapper.Entity.B, doubleLightShape);
            FlashlightUIManager.Instance.SetFlashlightCount(3);
        }

        private void Update()
        {
            if (FlashlightUIManager.Instance.flashlightCount <= 0)
                this.enabled = false;
        }

        private void OnDisable()
        {
            TilemapSwapper.Instance.SelectLightshape(TilemapSwapper.Entity.A, singleLightShape);
            TilemapSwapper.Instance.SelectLightshape(TilemapSwapper.Entity.B, singleLightShape);
        }
    }
}