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

    // 낙하 시간
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

        // 땅일 경우와 공중일 경우 애니메이션 지정
    }

    private void Jump()
    {
        if (isGrounded)
        {
            fallingTime = 0;

            // 점프 애니메이션 정지
            // 낙하 애니메이션 정지

            // 땅에 착지했을 경우 낙하 중지
            if (verticalSpeed < 0.0f)
                verticalSpeed = -2f;

            // 점프 실행
            if(pValue.jump)
            {
                verticalSpeed = Mathf.Sqrt(pStat.jumpheight * -2f * pStat.gravity);

                // 점프 애니메이션 실행
            }
        }
        else
        {
            // 체공 시간 계산
            fallingTime += Time.deltaTime;

            if(fallingTime >= 0.5f)
            {
                //낙하 애니메이션 실행
            }

            // 공중일 경우 점프 불가능
            pValue.jump = false;
        }

        if (verticalSpeed < pStat.fallingVelocity)
            verticalSpeed += pStat.gravity * Time.deltaTime;
    }

    private void MoveAndRotate()
    {
        #region Move

        // 이동 속도 연산 (기본 속도 / 달리기 속도)
        float targetSpeed = pValue.sprint ? pStat.movespeed * 2f : pStat.movespeed;
 
        // 입력 값이 없는 경우 이동 정지
        if(pValue.move == Vector2.zero)
            targetSpeed = 0;

        // 현재 이동중인 속도
        float currentSpeed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        // 이동 속도 연산 오프셋
        float speedOffset = 0.1f;

        // 현재 이동하고 있는 속도가 입력된 속도와 차이가 있는 경우 가속, 감속 연산 실행
        if (currentSpeed < targetSpeed - speedOffset ||
            currentSpeed > targetSpeed + speedOffset)
        {
            this.speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * pStat.acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        // 아닐 경우 입력된 이동 속도로 초기화
        else
            speed = targetSpeed;

        // 애니메이션 blend 수치 조정
        moveBlend = Mathf.Lerp(moveBlend,targetSpeed,Time.deltaTime*pStat.acceleration);
        if (moveBlend < 0.01f) moveBlend = 0f;

        #endregion


        #region Rotate

        // 현재 입력된 이동 방향 정규화
        Vector3 inputDir = new Vector3(pValue.move.x,0f,pValue.move.y).normalized;

        // 입력 값이 0이 아닌 경우 회전 방향 설정
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

        // 캐릭터 animator 파라미터 수정
    }

    private void CamRotate()
    {

    }
}
