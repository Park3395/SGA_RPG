using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    CharacterController cc;
    PlayerInputValue pValue;
    PlayerStatus pStat;
    
    // ���� ���� ����ִ��� �˻�
    private bool isGrounded = true;
    // ������ �ν��� ���̾�
    public LayerMask groundLayer;

    // �̵� �ӵ�
    private float speed = 2.0f;
    // �̵� �ִϸ��̼� ���� 
    private float moveBlend = 0f;
    // ���� �̵� �ӵ� (���� & ���� �ӵ�)
    private float verticalSpeed;

    // ȸ�� ����
    private float rotation;
    // ȸ�� �ӵ�
    private float rotSpeed;

    // ���� �ð�
    private float fallingTime;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        pValue = GetComponent<PlayerInputValue>();
        pStat = GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        Jump();
        GroundCheck();
        MoveAndRotate();
    }

    private void LateUpdate()
    {
        CamRotate();
    }

    private void GroundCheck()
    {
        Vector3 spherepos = new Vector3(this.transform.position.x, 
            this.transform.position.y - pStat.groundOffeset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherepos, pStat.groundRadius, groundLayer, 
            QueryTriggerInteraction.Ignore);

        // ���� ���� ������ ��� �ִϸ��̼� ����
    }

    private void Jump()
    {
        if (isGrounded)
        {
            fallingTime = 0;

            // ���� �ִϸ��̼� ����
            // ���� �ִϸ��̼� ����

            // ���� �������� ��� ���� ����
            if (verticalSpeed < 0.0f)
                verticalSpeed = -2f;

            // ���� ����
            if(pValue.jump)
            {
                verticalSpeed = Mathf.Sqrt(pStat.jumpheight * -2f * pStat.gravity);

                // ���� �ִϸ��̼� ����
            }
        }
        else
        {
            // ü�� �ð� ���
            fallingTime += Time.deltaTime;

            if(fallingTime >= 0.5f)
            {
                //���� �ִϸ��̼� ����
            }

            // ������ ��� ���� �Ұ���
            pValue.jump = false;
        }

        if (verticalSpeed < pStat.fallingVelocity)
            verticalSpeed += pStat.gravity * Time.deltaTime;
    }

    private void MoveAndRotate()
    {
        #region Move

        // �̵� �ӵ� ���� (�⺻ �ӵ� / �޸��� �ӵ�)
        float targetSpeed = pValue.sprint ? pStat.movespeed * 2f : pStat.movespeed;
 
        // �Է� ���� ���� ��� �̵� ����
        if(pValue.move == Vector2.zero)
            targetSpeed = 0;

        // ���� �̵����� �ӵ�
        float currentSpeed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        // �̵� �ӵ� ���� ������
        float speedOffset = 0.1f;

        // ���� �̵��ϰ� �ִ� �ӵ��� �Էµ� �ӵ��� ���̰� �ִ� ��� ����, ���� ���� ����
        if (currentSpeed < targetSpeed - speedOffset ||
            currentSpeed > targetSpeed + speedOffset)
        {
            this.speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * pStat.acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        // �ƴ� ��� �Էµ� �̵� �ӵ��� �ʱ�ȭ
        else
            speed = targetSpeed;

        // �ִϸ��̼� blend ��ġ ����
        moveBlend = Mathf.Lerp(moveBlend,targetSpeed,Time.deltaTime*pStat.acceleration);
        if (moveBlend < 0.01f) moveBlend = 0f;

        #endregion


        #region Rotate

        // ���� �Էµ� �̵� ���� ����ȭ
        Vector3 inputDir = new Vector3(pValue.move.x,0f,pValue.move.y).normalized;

        // �Է� ���� 0�� �ƴ� ��� ȸ�� ���� ����
        if(pValue.move != Vector2.zero)
        {
            rotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg 
                + Camera.main.transform.eulerAngles.y;
            float nowrot = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, rotation, ref rotSpeed, pStat.rotSmoothTime);

            transform.rotation = Quaternion.Euler(0f,nowrot,0);
        }

        Vector3 targetDir = Quaternion.Euler(0f, rotation, 0f) * Vector3.forward;

        #endregion

        cc.Move(targetDir.normalized * (speed * Time.deltaTime)
            + new Vector3(0f, verticalSpeed, 0f) * Time.deltaTime);

        // ĳ���� animator �Ķ���� ����
    }

    private void CamRotate()
    {

    }
}
