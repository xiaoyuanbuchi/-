using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Sign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public GameObject signSprite;
    private bool canPress;
    public Transform playerTrans;
    private IInteractable targetItem;

    private void Awake()
    {
        // anim = GetComponentInChildren<Animator>();
        anim = signSprite.GetComponent<Animator>();//获得子物体
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if(canPress)
        {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.PlayAudioClip();//播放打开宝箱音效
        }
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
       if(actionChange==InputActionChange.ActionStarted)
        {
            // Debug.Log(((InputAction)obj).activeControl.device);
            var d = ((InputAction)obj).activeControl.device;

            switch(d.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                    //如果要添加其他的输入设备，可以case xxx
            }
        }
    }

    private void OnDisable()
    {
        canPress = false;
    }
    private void Update()
    {
        // signSprite.SetActive(canPress);
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;//碰到宝箱时才启动e按键的spriteRenderer组件
        signSprite.transform.localScale = playerTrans.localScale;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Interactable"))
        {
            canPress = true;
            
            targetItem = other.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPress = false;
    }
}
