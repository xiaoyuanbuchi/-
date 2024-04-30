/*
 * CentralControl.cs
 * @brief: ���ڴ�����Ϸ��ȫ�ֿ��Ʒַ���
 * �������п��Ʋ������ڿ�ʼ����������ͣ��������
 * �����󽫲�����Ϣ�ַ�����ͬģ�顣
 * @author: YukinaSora
 * @date: 2023.10.16
 * @version: 0.0.2
 * 
 * --------------------
 * 2023.9.27 v0.0.1 YukinaSora
 * 1.�����ҿ��ƵĲ�������
 * --------------------
 * 2023.10.16 v0.0.2 YukinaSora
 * 1.��ӶԽ����Ի��Ĵ���
 * --------------------
 * 2023.10.23 v0.0.3 YukinaSora
 * 1.��ɶԽ����Ի��Ĵ���
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class CentralControl : MonoBehaviour
{
    private PlayerInputControl input;
    private PlayerController currentPlayer;
    private Dialog currentDialog;

    public bool isInteracting = false;

    public enum ControlState
    {
        Player,
        Dialog,
        Menu
    }
    private static ControlState state = ControlState.Menu;

    public void Awake()
    {
        input = new PlayerInputControl();
        input.Gameplay.Slide.started += handSlide;
        input.Gameplay.Jump.started += handleJump;
        input.Gameplay.Attack.started += handleAttact;
        input.Gameplay.WalkButton.performed += handleWalk;
        input.Gameplay.WalkButton.canceled += handleRun;
        input.Gameplay.Interact.performed += handleInteract;
        input.Gameplay.Drink.performed += drinkPotion;
        input.Gameplay.Drink.canceled += drinkDone;
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        input.Gameplay.Disable();
    }

    void Update()
    {
        currentPlayer = Global.CurrentPlayer();
        if (currentPlayer == null)
            return;
        handleMove();
    }

    public static void SetState(ControlState state)
    {
        CentralControl.state = state;
    }

    private void handleJump(CallbackContext ctx)
    {
        //Debug.Log("Jump");
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;

        currentPlayer.Jump(ctx);
    }

    private void handleAttact(CallbackContext ctx)
    {
        //Debug.Log("Attact");
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;

        currentPlayer.PlayerAttack(ctx);
    }

    private void handleRun(CallbackContext ctx)
    {
        //Debug.Log("Run");
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;

        currentPlayer.toRun();
    }

    private void handleWalk(CallbackContext ctx)
    {
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;

        currentPlayer.toWalk();
    }
    private void handSlide(CallbackContext ctx)
    {
        if (state != ControlState.Player)
            return;

        if (currentPlayer.isDead)
            return;
        currentPlayer.Slide(ctx);
    }

    private void handleMove()
    {
        //Debug.Log("Move");
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;

        Vector2 inputDirection = input.Gameplay.Move.ReadValue<Vector2>();
        currentPlayer.Move(inputDirection);
        currentPlayer.CheckState(inputDirection);
    }

    private void drinkPotion(CallbackContext context)
    {
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;
        currentPlayer.isDrinking = true;
        currentPlayer.DrinkPotion();
    }

    private void drinkDone(CallbackContext context)
    {
        if (state != ControlState.Player)
            return;

        bool returnFlag = currentPlayer.isDead || isInteracting;
        if (returnFlag)
            return;
        currentPlayer.toRun();
        currentPlayer.isDrinking = false;
    }

    private void handleInteract(CallbackContext ctx)
    {
        //Debug.Log("Interact");
        if (state == ControlState.Dialog)
        {
            StartCoroutine(NextDialog());
            return;
        }
        else if (currentPlayer.PlayerInteract())
        {
            StartCoroutine(Dialog());
        }
        // isInteracting = true;
    }

    private IEnumerator Dialog()
    {
        state = ControlState.Dialog;
        //Debug.Log("Dialog Mode");
        AsyncOperation operation = SceneManager.LoadSceneAsync("Dialog", LoadSceneMode.Additive);
        yield return operation;

        Scene scene = SceneManager.GetSceneByName("Dialog");
        currentDialog = GameObject.Find("Dialog").GetComponent<Dialog>();
        InteractableObject interactableObject = currentPlayer.interactCheck.Target.GetComponent<InteractableObject>();

        Vector3 positionPlayer = currentPlayer.transform.position + new Vector3(0, 2.5f, 0);
        Vector3 positionTarget = currentPlayer.interactCheck.Target.transform.position + new Vector3(0, 2.5f, 0);
        Vector3[] positions = { positionPlayer, positionTarget };
        currentDialog.SetPositions(positions);
        currentDialog.SetCallback(interactableObject.Callback);
        currentDialog.Load(interactableObject.ScriptPath());
        currentDialog.Next();
        // Debug.Log(currentPlayer.transform.position + new Vector3(0, 1, 0));
    }

    private IEnumerator NextDialog()
    {
        //Debug.Log("Player Mode");

        currentDialog.Next();

        if (currentDialog.Finished)
        {
            state = ControlState.Player;
            SceneManager.UnloadSceneAsync("Dialog");

            // -------------------------------------------------
            // �Ի��ڴ˽���������ӽ����¼���ص��������л��籾
            // AnyCallbackHere();
            currentDialog.Callback();
        }

        AsyncOperation operation = null;
        yield return operation;
    }
}
