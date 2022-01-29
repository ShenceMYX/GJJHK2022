using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class SoulUIManager : MonoSingleton<SoulUIManager>
	{
		public int soulCount { get; private set; }

		private Text soulCountText;

        private void Start()
        {
			soulCountText = GetComponentInChildren<Text>();
        }

		public void ChangeSoulAmount(int amount)
        {
			soulCount += amount;
			soulCountText.text = soulCount.ToString();
        }
    }
}