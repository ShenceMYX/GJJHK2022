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
        public bool cantMove = false;

		private GameObject[] players;
        private CharacterMotor[] motors;
        private CharacterInputController[] inputControllers;

        public override void Init()
        {
            base.Init();
            players = new GameObject[transform.childCount];
            motors = new CharacterMotor[2];
            inputControllers = new CharacterInputController[2];
            for (int i = 0; i < transform.childCount; i++)
            {
                players[i] = transform.GetChild(i).gameObject;
                motors[i] = players[i].GetComponent<CharacterMotor>();
                inputControllers[i] = players[i].GetComponent<CharacterInputController>();
            }
        }

        private void Update()
        {
            canInput = motors[0].reachTarget & motors[1].reachTarget;
        }

        public void DisablePlayers()
        {
            foreach (var player in players)
            {
                player.SetActive(false);
            }
        }

        public void ResetPressCount()
        {
            inputControllers[0].pressCount = inputControllers[1].pressCount = 0;
        }
    }
}