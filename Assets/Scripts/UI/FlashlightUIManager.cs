using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class FlashlightUIManager : MonoSingleton<FlashlightUIManager>
	{
		private GameObject[] flashlightUIs;

        public int flashlightCount { get; private set; }

        private void Start()
        {
			flashlightUIs = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                flashlightUIs[i] = transform.GetChild(i).gameObject;
            }

        }

        private void Update()
        {
            Debug.Log(flashlightCount);
            for (int i = 0; i < flashlightUIs.Length; i++)
            {
                if (i < flashlightCount)
                    flashlightUIs[i].SetActive(true);
                else
                    flashlightUIs[i].SetActive(false);

            }
        }

        public void SetFlashlightCount(int amount)
        {
            flashlightCount = amount;
        }

        public void DecreaseFlashlight()
        {
            flashlightCount--;
        }
    }
}