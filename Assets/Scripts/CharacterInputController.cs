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

        public KeyCode[] movementKeys = new KeyCode[4] { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S };

        public TilemapSwapper.Entity entityType = TilemapSwapper.Entity.B;
        public TilemapSwapper.Direction facingDirection;

        private bool isFlashlightOpened = false;
        public KeyCode flashlightTrigger = KeyCode.LeftShift;

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
            // Update Movement Key
            for (int i = 0; i < movementKeys.Length; i++)
            {
                if (Input.GetKeyDown(movementKeys[i]))
                {
                    switch (i)
                    {
                        //(int)KeyCode.A
                        case 0:
                            motor.Movement(new Vector2(-1, 0));
                            facingDirection = TilemapSwapper.Direction.LEFT;
                            //if(isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.LEFT);
                            break;
                        //(int)KeyCode.D
                        case 1:
                            motor.Movement(new Vector2(1, 0));
                            facingDirection = TilemapSwapper.Direction.RIGHT;
                            //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.RIGHT);
                            break;
                        //(int)KeyCode.W
                        case 2:
                            motor.Movement(new Vector2(0, 1));
                            facingDirection = TilemapSwapper.Direction.UP;
                            //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.UP);
                            break;
                        //(int)KeyCode.S
                        case 3:
                            motor.Movement(new Vector2(0, -1));
                            facingDirection = TilemapSwapper.Direction.DOWN;
                            //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.DOWN);
                            break;
                        default:
                            break;
                    }
                }
               
            }

            
        }

        private void FlashlightControlDetection()
        {
            if (Input.GetKeyDown(flashlightTrigger))
            {
                isFlashlightOpened = !isFlashlightOpened;
                if(isFlashlightOpened)
                    TilemapSwapper.Instance.ChangeTilemap(entityType, facingDirection);
            }
        }
    }
}