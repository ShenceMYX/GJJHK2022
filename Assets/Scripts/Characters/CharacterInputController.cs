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
        private GameObject[] lightSps;
        private GameObject[] doubleRangeLightSps;

        public KeyCode[] movementKeys = new KeyCode[4] { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S };

        public TilemapSwapper.Entity entityType = TilemapSwapper.Entity.B;
        public TilemapSwapper.Direction facingDirection;
        private TilemapSwapper.Direction initialFacingDirection;

        private bool isFlashlightOpened = false;
        public KeyCode flashlightKey = KeyCode.LeftShift;

        private float startPressTime;
        public float pressDuration = 1f;
        public int pressCount = 0;
        private float lastPressTime;

        public event Func<TilemapSwapper.Entity, Vector2, bool> isAboutToMoveHandler;

        public MMFeedbacks turnOnFlashlightFeedbacks;
        public MMFeedbacks turnOffFlashlightFeedbacks;

        public MMFeedbacks turnOnDoubleFlashlightFeedbacks;
        public MMFeedbacks turnOffDoubleFlashlightFeedbacks;

        private void Awake()
        {
            motor = GetComponent<CharacterMotor>();
            characterAnim = transform.GetChild(0).GetComponent<Animator>();
            initialFacingDirection = facingDirection;

            lightSps = new GameObject[4];
            Transform lightTF = transform.Find("light");
            if (lightTF != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    lightSps[i] = lightTF.GetChild(i).gameObject;
                }
            }

            doubleRangeLightSps = new GameObject[4];
            Transform doubleLightTF = transform.Find("doublelight");
            if (doubleLightTF != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    doubleRangeLightSps[i] = doubleLightTF.GetChild(i).gameObject;
                }
            }
        }

        private void OnEnable()
        {
            SetAnimationState();
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
            // Update Movement Key
            for (int i = 0; i < movementKeys.Length; i++)
            {
                if (Input.GetKey(movementKeys[i]))
                {
                    if (isFlashlightOpened)
                    {
                        TilemapSwapper.Instance.RestoreTilemap(entityType, facingDirection);
                        if (isFlashlightOpened)
                        {
                            turnOffFlashlightFeedbacks?.PlayFeedbacks();
                            isFlashlightOpened = false;
                        }
                        characterAnim.SetBool("flashlight", false);
                        if (FlashlightUIManager.Instance.flashlightCount > 0)
                        {
                            foreach (var light in doubleRangeLightSps)
                            {
                                light.SetActive(false);
                            }
                        }
                        else
                        {
                            foreach (var light in lightSps)
                            {
                                light.SetActive(false);
                            }
                        }
                        

                    }

                    if (motor.reachTarget)
                    {
                        startPressTime += Time.deltaTime;
                        switch (i)
                        {
                            //(int)KeyCode.A
                            case 0:
                                if(facingDirection == TilemapSwapper.Direction.LEFT)
                                {
                                    Movement(new Vector2(-1, 0));

                                }
                                facingDirection = TilemapSwapper.Direction.LEFT;
                                //if(isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.LEFT);
                                break;
                            //(int)KeyCode.D
                            case 1:
                                if (facingDirection == TilemapSwapper.Direction.RIGHT)
                                {
                                    Movement(new Vector2(1, 0));

                                }
                                facingDirection = TilemapSwapper.Direction.RIGHT;
                                //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.RIGHT);
                                break;
                            //(int)KeyCode.W
                            case 2:
                                if (facingDirection == TilemapSwapper.Direction.UP)
                                {
                                    Movement(new Vector2(0, 1));

                                }
                                facingDirection = TilemapSwapper.Direction.UP;
                                //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.UP);
                                break;
                            //(int)KeyCode.S
                            case 3:
                                if (facingDirection == TilemapSwapper.Direction.DOWN)
                                {
                                    Movement(new Vector2(0, -1));

                                }
                                facingDirection = TilemapSwapper.Direction.DOWN;
                                //if (isFlashlightOpened) TilemapSwapper.Instance.ChangeTilemap(entityType, TilemapSwapper.Direction.DOWN);
                                break;
                            default:
                                break;
                        }

                    }

                    SetAnimationState();
                    //TilemapSwapper.Instance.UpdateTilemap(entityType);

                    lastPressTime = Time.time;
                }

            }

            if (Time.time - lastPressTime > 0.1)
                pressCount = 0;

            
        }

        private void Movement(Vector2 dir)
        {
            if (!isAboutToMoveHandler(entityType, dir)) return;

            if (pressCount > 0)
            {
                motor.Movement(dir);
                pressCount++;
                OnPlayerMove();
                startPressTime = 0;
            }
            else
            {
                if (startPressTime > pressDuration)
                {
                    motor.Movement(dir);
                    pressCount++;
                    OnPlayerMove();
                    startPressTime = 0;
                }
            }
        }

        private void OnPlayerMove()
        {
            //CheckPortal();
            //CheckDoor();
        }

        

        
        private void FlashlightControlDetection()
        {
            if (!motor.reachTarget) return;
            if (Input.GetKeyDown(flashlightKey))
            {
                isFlashlightOpened = !isFlashlightOpened;
                characterAnim.SetBool("flashlight", isFlashlightOpened);
                int flashlightIndex = 0;
                Vector2 dir = Vector2.zero;
                switch (facingDirection)
                {
                    case TilemapSwapper.Direction.UP:
                        flashlightIndex = 0;
                        dir = new Vector2(0, 1);
                        break;
                    case TilemapSwapper.Direction.DOWN:
                        flashlightIndex = 1;
                        dir = new Vector2(0, -1);
                        break;
                    case TilemapSwapper.Direction.LEFT:
                        flashlightIndex = 2;
                        dir = new Vector2(-1, 0);
                        break;
                    case TilemapSwapper.Direction.RIGHT:
                        flashlightIndex = 3;
                        dir = new Vector2(1, 0);
                        break;
                    default:
                        break;
                }

                if (FlashlightUIManager.Instance.flashlightCount > 0)
                    doubleRangeLightSps[flashlightIndex].SetActive(isFlashlightOpened);
                else
                    lightSps[flashlightIndex].SetActive(isFlashlightOpened);

                if (isFlashlightOpened)
                {
                    TilemapSwapper.Instance.ChangeTilemap(entityType, facingDirection);

                    if (FlashlightUIManager.Instance.flashlightCount > 0)
                    {
                        turnOnDoubleFlashlightFeedbacks?.PlayFeedbacks(transform.position + new Vector3(dir.x * 2, dir.y * 2 - 0.73f, 0));
                    }

                    turnOnFlashlightFeedbacks?.PlayFeedbacks(transform.position + new Vector3(dir.x, dir.y - 0.73f, 0));

                }
                else
                {
                    TilemapSwapper.Instance.RestoreTilemap(entityType, facingDirection);

                    if (FlashlightUIManager.Instance.flashlightCount > 0)
                    {
                        FlashlightUIManager.Instance.DecreaseFlashlight();
                        turnOffDoubleFlashlightFeedbacks?.PlayFeedbacks();
                    }
                    
                    turnOffFlashlightFeedbacks?.PlayFeedbacks();

                }
            }
        }

        private void SetAnimationState()
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
            SetAnimationState();
        }

    }
}