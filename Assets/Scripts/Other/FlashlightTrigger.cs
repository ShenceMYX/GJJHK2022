using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class FlashlightTrigger : MonoBehaviour
	{
        public GameObject flashlightUI;
        private int requireSoulAmount = 2;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && SoulUIManager.Instance.soulCount >= requireSoulAmount) 
            {
                SoulUIManager.Instance.ChangeSoulAmount(-2);

                flashlightUI.SetActive(true);

                PlayerInstance.Instance.GetComponent<FlashlightController>().enabled = true;

                gameObject.SetActive(false);
            }
        }
    }
}