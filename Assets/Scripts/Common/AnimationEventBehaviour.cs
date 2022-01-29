using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ns;

namespace Common
{
    ///<summary>
    ///动画事件行为类：在特定时机取消动画，在特定时机引发动画事件
    ///</summary>
    public class AnimationEventBehaviour : MonoBehaviour
    {
        //策划：为动画片段添加事件，指向OnCancelAnim, OnAttack
        //程序：在脚本中播放动画，动画中需要执行的逻辑，注册attackHandler事件

        private Animator anim;

        public event Action attackHandler;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        //由Unity引擎调用
        private void OnCancelAnim(string animParam)
        {
            anim.SetBool(animParam, false);
        }

        //由Unity引擎调用
        private void OnAttack()
        {
            attackHandler?.Invoke();//引发事件
        }

        private void OnDisableGameObject()
        {
            MapController.Instance.ResetMapAndPlayerPos();
            PlayerInstance.Instance.ResetPressCount();
            PlayerInstance.Instance.cantMove = false;
            this.gameObject.SetActive(false);
        }
    }
}
