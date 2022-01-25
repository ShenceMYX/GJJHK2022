using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class DieMenu : MonoSingleton<DieMenu>
	{
		public GameObject dieMenu;

		public void PauseGame()
        {
			dieMenu.SetActive(true);
			Time.timeScale = 0;
        }

		public void RestartGame()
        {
			dieMenu.SetActive(false);
			Time.timeScale = 1;
			MapController.Instance.ResetMapAndPlayerPos();
		}

		public void QuitGame()
        {
			Application.Quit();
        }
    }
}