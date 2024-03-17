using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputValue : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool dodge;

    public bool UIOpened;
    public bool exitUI;

    public bool norAtk;
    public bool chgAtk;
    public bool spcAtk;
    public bool ultAtk;

    public bool LockedCursor = true;
    public bool onAction = false;

    public void StopInput()
    {
        move = Vector2.zero;
        look = Vector2.zero;
        jump = false;
        sprint = false;
        dodge = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jump = true;
        else
            jump = false;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = !sprint;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
            dodge = true;
        else
            dodge = false;
    }

    public void OnTap(InputAction.CallbackContext context)
    {
        if (context.performed)
            UIOpened = true;
        else
            UIOpened = false;

        if (context.canceled)
            exitUI = true;
        else
            exitUI = false;
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            norAtk = true;
        else
            norAtk = false;
    }

    public void OnCharge(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            chgAtk = true;
        }
        else
            chgAtk = false;
    }

    public void onRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            spcAtk = true;
        else
            spcAtk = false;
    }

    public void onUlt(InputAction.CallbackContext context)
    {
        if (context.performed)
            ultAtk = true;
        else
            ultAtk = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = LockedCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
