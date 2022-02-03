using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class Test : MonoBehaviour
	{
        public float sp = 5;
        private Camera cam;
        private void Start()
        {
            cam = Camera.main;
        }
        private void Update()
        {
            transform.Translate(sp * Time.deltaTime, 0, 0);
            cam.orthographicSize += Time.deltaTime;
        }
    }
}