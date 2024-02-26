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

    // 현재 땅에 닿아있는지 검사
    private bool isGrounded = true;
    // 땅으로 인식할 레이어
    public LayerMask groundLayer;

    // 이동 속도
    private float speed = 2.0f;
    // 이동 애니메이션 조정 
    private float moveBlend = 0f;
    // 수직 이동 속도 (점프 & 낙하 속도)
    private float verticalSpeed;

    // 회전 방향
    private float rotation;
    // 회전 속도
    private float rotSpeed;


    public bool jumpDelay;
    // 낙하 시간
    private float fallingTime;
    // 낙하 데미지 최소 값
    private float minFallingDamageTime = 3f;

    // 회피 트리거
    public bool isDodge = false;
    // 회피 쿨타임
    private float dodgeDelay;


    // 스레드홀드
    private float threshold = 0.01f;

    // 카메라 y 각
    private float camTargetY;
    // 카메라 pitch
    private float camTargetPitch;
    // 카메라 타겟
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

            // 점프 & 낙하 애니메이션 중지
            pAnim.SetBool("Jump", false);
            pAnim.SetBool("Falling", false);

            // 땅에 착지했을 경우 낙하 중지
            if (verticalSpeed < 0.0f)
                verticalSpeed = -2f;

            // 점프 실행
            if(pValue.jump && !jumpDelay)
            {
                verticalSpeed = Mathf.Sqrt(pStat.jumpheight * -2f * pStat.gravity);

                jumpDelay = true;

                // 점프 애니메이션 실행
                pAnim.SetBool("Jump", true);
            }
        }
        else
        {
            // 체공 시간 계산
            fallingTime += Time.deltaTime;

            if(fallingTime >= 0.3f)
            {
                //낙하 애니메이션 실행
                pAnim.SetBool("Falling", true);
            }
            if(fallingTime >= minFallingDamageTime)
            {
                pAnim.SetBool("LargeFall", true);
            }

            // 공중일 경우 점프 불가능
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
            // 회피중인 경우 점프 불가능
            pValue.jump = false;
            // 회피중인 경우 충돌 판정 무시
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

        // 이동 속도 연산 (기본 속도 / 달리기 속도)
        float targetSpeed = pValue.sprint ? pStat.movespeed * 2f : pStat.movespeed;
 
        // 입력 값이 없는 경우 이동 정지
        if(pValue.move == Vector2.zero)
            targetSpeed = 0f;

        // 현재 이동중인 속도
        float currentSpeed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        // 이동 속도 연산 오프셋
        float speedOffset = 0.1f;
        float inputMagnitude = 1.0f;

        // 현재 이동하고 있는 속도가 입력된 속도와 차이가 있는 경우 가속, 감속 연산 실행
        if (currentSpeed < targetSpeed - speedOffset ||
            currentSpeed > targetSpeed + speedOffset)
        {
            this.speed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude
                , isDodge ? 0.05f : Time.deltaTime * pStat.acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        // 아닐 경우 입력된 이동 속도로 초기화
        else
        {
            speed = targetSpeed;
        }

        Dodge();

        // 애니메이션 blend 수치 조정
        moveBlend = Mathf.Lerp(moveBlend, targetSpeed, Time.deltaTime * pStat.acceleration);
        if (moveBlend < 0.01f) moveBlend = 0f;

        #endregion


        #region Rotate

        // 현재 입력된 이동 방향 정규화
        Vector3 inputDir = new Vector3(pValue.move.x,0f,pValue.move.y).normalized;

        // 입력 값이 0이 아닌 경우 회전 방향 설정
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

        // 캐릭터 animator 파라미터 수정
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
