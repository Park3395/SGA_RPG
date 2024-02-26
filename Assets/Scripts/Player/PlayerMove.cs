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
    Animator pAnim;

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


    public bool jumpDelay;
    // ���� �ð�
    private float fallingTime;
    // ���� ������ �ּ� ��
    private float minFallingDamageTime = 3f;

    // ȸ�� Ʈ����
    public bool isDodge = false;
    // ȸ�� ��Ÿ��
    private float dodgeDelay;


    // ������Ȧ��
    private float threshold = 0.01f;

    // ī�޶� y ��
    private float camTargetY;
    // ī�޶� pitch
    private float camTargetPitch;
    // ī�޶� Ÿ��
    public GameObject camTarget;

    public bool isStopped = false;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        pValue = GetComponent<PlayerInputValue>();
        pStat = GetComponent<PlayerStatus>();
        pAnim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        StopCheck();
        Jump();
        GroundCheck();
        MoveAndRotate();
    }

    private void LateUpdate()
    {
        CamRotate();
    }

    private void StopCheck()
    {
        if(isStopped)
        {
            pValue.StopInput();
        }
    }

    private void GroundCheck()
    {
        Vector3 spherepos = new Vector3(this.transform.position.x,
            this.transform.position.y - pStat.groundOffeset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherepos, pStat.groundRadius, groundLayer,
            QueryTriggerInteraction.Ignore);

        if (isGrounded)
        {
            if(fallingTime >= minFallingDamageTime)
            {
                this.isStopped = true;
            }
        }

        pAnim.SetBool("Grounded",isGrounded);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            fallingTime = 0;

            // ���� & ���� �ִϸ��̼� ����
            pAnim.SetBool("Jump", false);
            pAnim.SetBool("Falling", false);

            // ���� �������� ��� ���� ����
            if (verticalSpeed < 0.0f)
                verticalSpeed = -2f;

            // ���� ����
            if(pValue.jump && !jumpDelay)
            {
                verticalSpeed = Mathf.Sqrt(pStat.jumpheight * -2f * pStat.gravity);

                jumpDelay = true;

                // ���� �ִϸ��̼� ����
                pAnim.SetBool("Jump", true);
            }
        }
        else
        {
            // ü�� �ð� ���
            fallingTime += Time.deltaTime;

            if(fallingTime >= 0.3f)
            {
                //���� �ִϸ��̼� ����
                pAnim.SetBool("Falling", true);
            }
            if(fallingTime >= minFallingDamageTime)
            {
                pAnim.SetBool("LargeFall", true);
            }

            // ������ ��� ���� �Ұ���
            pValue.jump = false;
        }

        if (verticalSpeed < pStat.fallingVelocity)
            verticalSpeed += pStat.gravity * Time.deltaTime;
    }

    private void Dodge()
    {
        if(dodgeDelay != 0)
            dodgeDelay -= Time.deltaTime;

        pAnim.SetBool("inDodge", false);

        if(isGrounded && !jumpDelay)
        {
            if(pValue.dodge & dodgeDelay <= 0)
            {
                if (!isDodge)
                {
                    speed = pStat.movespeed * 8f;
                    
                    isDodge = true;
                    jumpDelay = true;
                    dodgeDelay = pStat.dodgeDelay;

                    pAnim.SetBool("inDodge", true);
                }
            }
        }
        else
        {
                pAnim.SetBool("inDodge", pValue.dodge);
        }

        if(isDodge)
        {
            // ȸ������ ��� ���� �Ұ���
            pValue.jump = false;
            // ȸ������ ��� �浹 ���� ����
            cc.detectCollisions = false;
        }
        else
        {
            cc.detectCollisions = true;
        }
    }

    private void MoveAndRotate()
    {
        #region Move

        // �̵� �ӵ� ���� (�⺻ �ӵ� / �޸��� �ӵ�)
        float targetSpeed = pValue.sprint ? pStat.movespeed * 2f : pStat.movespeed;
 
        // �Է� ���� ���� ��� �̵� ����
        if(pValue.move == Vector2.zero)
            targetSpeed = 0f;

        // ���� �̵����� �ӵ�
        float currentSpeed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        // �̵� �ӵ� ���� ������
        float speedOffset = 0.1f;
        float inputMagnitude = 1.0f;

        // ���� �̵��ϰ� �ִ� �ӵ��� �Էµ� �ӵ��� ���̰� �ִ� ��� ����, ���� ���� ����
        if (currentSpeed < targetSpeed - speedOffset ||
            currentSpeed > targetSpeed + speedOffset)
        {
            this.speed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude
                , isDodge ? 0.05f : Time.deltaTime * pStat.acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        // �ƴ� ��� �Էµ� �̵� �ӵ��� �ʱ�ȭ
        else
        {
            speed = targetSpeed;
        }

        Dodge();

        // �ִϸ��̼� blend ��ġ ����
        moveBlend = Mathf.Lerp(moveBlend, targetSpeed, Time.deltaTime * pStat.acceleration);
        if (moveBlend < 0.01f) moveBlend = 0f;

        #endregion


        #region Rotate

        // ���� �Էµ� �̵� ���� ����ȭ
        Vector3 inputDir = new Vector3(pValue.move.x,0f,pValue.move.y).normalized;

        // �Է� ���� 0�� �ƴ� ��� ȸ�� ���� ����
        if(pValue.move != Vector2.zero && !isDodge)
        {
            rotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg 
                + Camera.main.transform.eulerAngles.y;
            float nowrot = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, rotation, 
                ref rotSpeed, pStat.rotSmoothTime);

            transform.rotation = Quaternion.Euler(0f, nowrot, 0f);
        }

        Vector3 targetDir = Quaternion.Euler(0f, rotation, 0f) * Vector3.forward;

        #endregion

        cc.Move(targetDir.normalized * (speed * Time.deltaTime)
            + new Vector3(0f, verticalSpeed, 0f) * Time.deltaTime);

        // ĳ���� animator �Ķ���� ����
        pAnim.SetFloat("Speed",moveBlend);
    }

    private void CamRotate()
    {
        if(pValue.look.sqrMagnitude >= threshold)
        {
            camTargetY += pValue.look.x * pStat.camSpeed;
            camTargetPitch += pValue.look.y * pStat.camSpeed;
        }

        camTargetY = ClampAngle(camTargetY,float.MinValue,float.MaxValue);
        camTargetPitch = ClampAngle(camTargetPitch,pStat.camBottomClamp,pStat.camTopClamp);

        camTarget.transform.rotation = Quaternion.Euler(camTargetPitch, camTargetY, 0f);
    }

    private static float ClampAngle(float angle,float min,float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
