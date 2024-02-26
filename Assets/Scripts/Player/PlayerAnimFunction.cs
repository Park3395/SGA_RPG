using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimFunction : MonoBehaviour
{
    PlayerMove pMove;

    private void Awake()
    {
        pMove = GetComponentInParent<PlayerMove>();
    }

    private void EndDodge()
    {
        pMove.isDodge = false;
    }

    private void jumpActive()
    {
        pMove.jumpDelay = false;
    }

    private void StartInput()
    {
        pMove.isStopped = false;
    }
}
