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
        private Animator anim;

        public KeyCode[] movementKeys = new KeyCode[4] { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S };

        public TilemapSwapper.Entity entityType = TilemapSwapper.Entity.B;
        public TilemapSwapper.Direction facingDirection;

        private bool isFlashlightOpened = false;
        public KeyCode flashlightKey = KeyCode.LeftShift;

        private void Awake()
        {
            motor = GetComponent<CharacterMotor>();
            anim = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            SetAnimationState();
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
                    if (isFlashlightOpened)
                    {
                        TilemapSwapper.Instance.RestoreTilemap(entityType, facingDirection);
                        isFlashlightOpened = false;
                    }
                    switch (i)
                    {
                        //(int)KeyCode.A
                        case 0:
                            if(facingDirection == TilemapSwapper.Direction.LEFT)
                                motor.Movement(new Vector2(-1, 0));
                            facingDirection = TilemapSwapper.Direction.LEFT;
                            //if(isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.LEFT);
                            break;
                        //(int)KeyCode.D
                        case 1:
                            if (facingDirection == TilemapSwapper.Direction.RIGHT)
                                motor.Movement(new Vector2(1, 0));
                            facingDirection = TilemapSwapper.Direction.RIGHT;
                            //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.RIGHT);
                            break;
                        //(int)KeyCode.W
                        case 2:
                            if (facingDirection == TilemapSwapper.Direction.UP)
                                motor.Movement(new Vector2(0, 1));
                            facingDirection = TilemapSwapper.Direction.UP;
                            //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.UP);
                            break;
                        //(int)KeyCode.S
                        case 3:
                            if (facingDirection == TilemapSwapper.Direction.DOWN)
                                motor.Movement(new Vector2(0, -1));
                            facingDirection = TilemapSwapper.Direction.DOWN;
                            //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.DOWN);
                            break;
                        default:
                            break;
                    }

                    SetAnimationState();

                }

            }

            
        }

        private void FlashlightControlDetection()
        {
            if (Input.GetKeyDown(flashlightKey))
            {
                isFlashlightOpened = !isFlashlightOpened;
                if (isFlashlightOpened)
                    TilemapSwapper.Instance.ChangeTilemap(entityType, facingDirection);
                else
                    TilemapSwapper.Instance.RestoreTilemap(entityType, facingDirection);
            }
        }

        private void SetAnimationState()
        {
            switch (facingDirection)
            {
                case TilemapSwapper.Direction.LEFT:
                    anim.SetFloat("Vertical", 0);
                    anim.SetFloat("Horizontal", -1);
                    break;
                case TilemapSwapper.Direction.RIGHT:
                    anim.SetFloat("Vertical", 0);
                    anim.SetFloat("Horizontal", 1);
                    break;
                case TilemapSwapper.Direction.UP:
                    anim.SetFloat("Horizontal", 0);
                    anim.SetFloat("Vertical", -1);
                    break;
                case TilemapSwapper.Direction.DOWN:
                    anim.SetFloat("Horizontal", 0);
                    anim.SetFloat("Vertical", 1);
                    break;
                default:
                    break;
            }
        }

    }
}