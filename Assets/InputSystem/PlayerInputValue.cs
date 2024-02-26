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

    public bool LockedCursor = true;

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

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = LockedCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
