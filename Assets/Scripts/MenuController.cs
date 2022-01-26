using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class MenuController : MonoSingleton<MenuController>
	{
		public GameObject dieMenu;
		public GameObject waitingMenu;
		public GameObject sceneTransitionUI;

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

		public void ShowWaitingUI()
        {
			waitingMenu.SetActive(true);
        }

		public void CloseWaitingUI()
		{
			waitingMenu.SetActive(false);
		}

		public void PlaySceneTransition()
        {
			sceneTransitionUI.SetActive(true);
        }
		
	}
}