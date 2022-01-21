using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class CharacterInputController : MonoBehaviour
	{
		private CharacterMotor motor;

        private void Start()
        {
			motor = GetComponent<CharacterMotor>();
        }

        private void Update()
        {
            MovementControlDetection();
            FlashlightControlDetection();
        }

        private void MovementControlDetection()
        {

        }

        private void FlashlightControlDetection()
        {

        }
    }
}