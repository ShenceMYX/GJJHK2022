using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class FlashlightController : MonoBehaviour
	{
		private Dictionary<TilemapSwapper.Direction, GameObject> lightSpritesDIC;
		private Dictionary<TilemapSwapper.Direction, GameObject> doubleRangeLightSpritesDIC;

		public bool isFlashlightOpened { get; private set; } = false;

		public MMFeedbacks turnOnFlashlightFeedbacks;
		public MMFeedbacks turnOffFlashlightFeedbacks;

		public MMFeedbacks turnOnDoubleFlashlightFeedbacks;
		public MMFeedbacks turnOffDoubleFlashlightFeedbacks;

		private Animator characterAnim;


		private void Awake()
        {
			characterAnim = GetComponentInChildren<Animator>();
			lightSpritesDIC = InitializeFlashlightSprtiesDIC("light");
			doubleRangeLightSpritesDIC = InitializeFlashlightSprtiesDIC("doublelight");
		}

		private Dictionary<TilemapSwapper.Direction, GameObject> InitializeFlashlightSprtiesDIC(string flashLightSpriteName)
		{
			Dictionary<TilemapSwapper.Direction, GameObject> lightSpritesDIC = new Dictionary<TilemapSwapper.Direction, GameObject>();
			Transform lightTF = transform.Find(flashLightSpriteName);
			if (lightTF != null)
			{
				for (int i = 0; i < 4; i++)
				{
					GameObject lightSpriteGO = lightTF.GetChild(i).gameObject;
					lightSpritesDIC.Add((TilemapSwapper.Direction)Enum.Parse(typeof(TilemapSwapper.Direction), lightSpriteGO.name), lightSpriteGO);
				}
			}

			return lightSpritesDIC;
		}

		public void HandleFlashlightOnOff(TilemapSwapper.Entity entityType, TilemapSwapper.Direction facingDirection)
		{
			Vector2 dir = TilemapSwapper.Instance.ConvertDirectionToVector2(facingDirection);

			isFlashlightOpened = !isFlashlightOpened;
			characterAnim.SetBool("flashlight", isFlashlightOpened);

			if (FlashlightUIManager.Instance.flashlightCount > 0)
				doubleRangeLightSpritesDIC[facingDirection].SetActive(isFlashlightOpened);
			else
				lightSpritesDIC[facingDirection].SetActive(isFlashlightOpened);

			if (isFlashlightOpened)
			{
				TilemapSwapper.Instance.ChangeTilemap(entityType, facingDirection);

				if (FlashlightUIManager.Instance.flashlightCount > 0)
				{
					turnOnDoubleFlashlightFeedbacks?.PlayFeedbacks(transform.position + new Vector3(dir.x * 2, dir.y * 2 - 0.73f, 0));
				}

				turnOnFlashlightFeedbacks?.PlayFeedbacks(transform.position + new Vector3(dir.x, dir.y - 0.73f, 0));

			}
			else
			{
				TilemapSwapper.Instance.RestoreTilemap(entityType, facingDirection);

				if (FlashlightUIManager.Instance.flashlightCount > 0)
				{
					FlashlightUIManager.Instance.DecreaseFlashlight();
					turnOffDoubleFlashlightFeedbacks?.PlayFeedbacks();
				}

				turnOffFlashlightFeedbacks?.PlayFeedbacks();

			}
		}

	}
}