using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayerInstance : MonoSingleton<PlayerInstance>
    {
		public bool canInput = true;

		private GameObject[] players;

        public override void Init()
        {
            base.Init();
            players = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                players[i] = transform.GetChild(i).gameObject;
            }
        }

        public void DisablePlayers()
        {
            foreach (var player in players)
            {
                player.SetActive(false);
            }
        }
    }
}