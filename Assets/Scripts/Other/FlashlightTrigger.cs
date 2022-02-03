using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
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

        public MMFeedbacks flashlightAcquireFeedbacks;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && SoulUIManager.Instance.soulCount >= requireSoulAmount) 
            {
                SoulUIManager.Instance.ChangeSoulAmount(-2);

                flashlightAcquireFeedbacks?.PlayFeedbacks();

                flashlightUI.SetActive(true);

                PlayerInstance.Instance.GetComponent<DoubleFlashlightController>().enabled = true;

                gameObject.SetActive(false);
            }
        }
    }
}