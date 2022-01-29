using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class SoulTrigger : MonoBehaviour
	{
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                SoulUIManager.Instance.ChangeSoulAmount(1);
                gameObject.SetActive(false);
            }
        }
    }
}