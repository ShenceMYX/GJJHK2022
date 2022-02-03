using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ns
{
	/// <summary>
	/// 
	/// </summary>
	public class CharacterInputController : MonoBehaviour
	{
		private CharacterMotor motor;
        private Animator characterAnim;
        

        public KeyCode[] movementKeys = new KeyCode[4] { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S };
        private Dictionary<TilemapSwapper.Direction, KeyCode> movementKeysDIC = new Dictionary<TilemapSwapper.Direction, KeyCode>
        {
            { TilemapSwapper.Direction.LEFT, KeyCode.A },
            { TilemapSwapper.Direction.RIGHT, KeyCode.D },
            { TilemapSwapper.Direction.UP, KeyCode.W },
            { TilemapSwapper.Direction.DOWN, KeyCode.S }
        };

        public KeyCode flashlightKey = KeyCode.LeftShift;

        public TilemapSwapper.Entity entityType = TilemapSwapper.Entity.B;
        public TilemapSwapper.Direction facingDirection;
        private TilemapSwapper.Direction initialFacingDirection;

        private float startPressTime;
        public float pressDuration = 1f;
        public int pressCount = 0;
        private float lastPressTime;

        public event Func<TilemapSwapper.Entity, Vector2, bool> isAboutToMoveHandler;

        private FlashlightController flashlightController;

        private void Awake()
        {
            motor = GetComponent<CharacterMotor>();
            flashlightController = GetComponent<FlashlightController>();
            characterAnim = transform.GetChild(0).GetComponent<Animator>();
            initialFacingDirection = facingDirection;
        }

        private void OnEnable()
        {
            UpdateAnimationState();
        }

        private void Update()
        {
            TilemapSwapper.Instance.UpdateTilemap(entityType);

            if (!PlayerInstance.Instance.canInput) return;
            MovementControlDetection();
            FlashlightControlDetection();

        }

        private void MovementControlDetection()
        {
            foreach (KeyValuePair<TilemapSwapper.Direction, KeyCode> pair in movementKeysDIC)
            {
                if (!Input.GetKey(pair.Value)) continue;

                if (flashlightController.isFlashlightOpened)
                    flashlightController.HandleFlashlightOnOff(entityType, facingDirection);

                HandleCharacterMovement(pair.Key);

                UpdateAnimationState();

                lastPressTime = Time.time;
            }

            if (Time.time - lastPressTime > 0.1)
                pressCount = 0;

            
        }

        private void HandleCharacterMovement(TilemapSwapper.Direction direction)
        {
            if (!motor.reachTarget) return;
            startPressTime += Time.deltaTime;

            if (facingDirection == direction)
                Movement(TilemapSwapper.Instance.ConvertDirectionToVector2(facingDirection));
            facingDirection = direction;
        }

        private void Movement(Vector2 dir)
        {
            if (!isAboutToMoveHandler(entityType, dir)) return;

            if (pressCount > 0)
            {
                motor.Movement(dir);
                pressCount++;
                startPressTime = 0;
            }
            else
            {
                if (startPressTime > pressDuration)
                {
                    motor.Movement(dir);
                    pressCount++;
                    startPressTime = 0;
                }
            }
        }

        
        private void FlashlightControlDetection()
        {
            if (!motor.reachTarget) return;
            if (!Input.GetKeyDown(flashlightKey)) return;

            flashlightController.HandleFlashlightOnOff(entityType, facingDirection);
        }


        private void UpdateAnimationState()
        {
            switch (facingDirection)
            {
                case TilemapSwapper.Direction.LEFT:
                    characterAnim.SetFloat("Vertical", 0);
                    characterAnim.SetFloat("Horizontal", -1);
                    break;
                case TilemapSwapper.Direction.RIGHT:
                    characterAnim.SetFloat("Vertical", 0);
                    characterAnim.SetFloat("Horizontal", 1);
                    break;
                case TilemapSwapper.Direction.UP:
                    characterAnim.SetFloat("Horizontal", 0);
                    characterAnim.SetFloat("Vertical", -1);
                    break;
                case TilemapSwapper.Direction.DOWN:
                    characterAnim.SetFloat("Horizontal", 0);
                    characterAnim.SetFloat("Vertical", 1);
                    break;
                default:
                    break;
            }
        }

        public void ResetFacingDir()
        {
            facingDirection = initialFacingDirection;
            UpdateAnimationState();
        }

    }
}