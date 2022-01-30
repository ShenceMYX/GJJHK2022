using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class SoulTrigger : MonoBehaviour
	{
        public MMFeedbacks itemAcquireFeedbacks;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                SoulUIManager.Instance.ChangeSoulAmount(1);
                itemAcquireFeedbacks?.PlayFeedbacks();
                gameObject.SetActive(false);
            }
        }
    }
}